using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Domain.Features.Security.Enums;
using NotaryPlatform.Infrastructure.Authorization.PermissionMaps;
using NotaryPlatform.Infrastructure.Persistence.Repositories.Core;
using NotaryPlatform.Infrastructure.Persistence.Repositories.Security;
using NotaryPlatform.Infrastructure.Services.Authentication;
using OtpNet;
using Xunit;

namespace NotaryPlatform.Infrastructure.Tests.Integration;

/// <summary>
/// End-to-end UC-AUTH-06 tests against a real PostgreSQL container + the source-of-truth schema. These
/// cover the DB-bound truths unit tests can't: real secret encryption at rest, verified-row state, hashed
/// recovery metadata, the BR-AUTH-05 login gate, and the EF multi-row insert / one-primary supersede.
/// </summary>
[Collection(PostgresCollection.Name)]
public sealed class MfaTotpIntegrationTests
{
    private readonly PostgresSchemaFixture _fx;
    private readonly TotpService _totp = new();
    private readonly RecoveryCodeService _recovery = new();
    private readonly IMfaSecretVault _vault;

    public MfaTotpIntegrationTests(PostgresSchemaFixture fx)
    {
        _fx = fx;
        var provider = new ServiceCollection().AddDataProtection().Services
            .BuildServiceProvider().GetRequiredService<IDataProtectionProvider>();
        _vault = new DataProtectionMfaSecretVault(provider);
    }

    [Fact] // TC-INT-01/02/03/05 — real round trip + encrypted secret + verified row state + hashed codes
    public async Task Enroll_then_verify_round_trip_persists_the_expected_state()
    {
        var (tenantId, userId) = await SeedUserAsync();

        var secret = _totp.GenerateSecret();
        var secretRef = _vault.Store(secret);
        var deviceId = await EnrollAsync(tenantId, userId, secretRef);

        // TC-INT-02 — what landed in secret_reference is the ciphertext, never the raw secret.
        await using (var read = _fx.CreateContext())
        {
            var stored = await read.MfaDevices.AsNoTracking().FirstAsync(m => m.MfaDeviceId == deviceId);
            stored.SecretReference.Should().Be(secretRef);
            stored.SecretReference.Should().NotBe(secret);
            stored.IsVerified.Should().BeFalse();
            stored.status.Should().Be(DeviceStatus.Pending);
        }

        // TC-INT-01 — a code computed from the returned secret validates, then activate + issue codes.
        var code = new Totp(Base32Encoding.ToBytes(secret)).ComputeTotp();
        _totp.ValidateCode(secret, code).Should().BeTrue();

        var rawCodes = _recovery.Generate(AppDefaultsRecoveryCount);
        var hashes = rawCodes.Select(_recovery.Hash).ToList();
        await VerifyAsync(deviceId, userId, tenantId, hashes);

        await using var ctx = _fx.CreateContext();
        var devices = await ctx.MfaDevices.AsNoTracking().Where(m => m.UserId == userId).ToListAsync();

        // TC-INT-03 — verified TOTP row state.
        var totp = devices.Single(d => d.method_type == MfaMethodType.Totp);
        totp.IsVerified.Should().BeTrue();
        totp.status.Should().Be(DeviceStatus.Trusted);
        totp.IsPrimary.Should().BeTrue();
        totp.VerifiedAt.Should().NotBeNull();

        // TC-INT-05 — recovery codes stored as hashes; the raw codes are NOT persisted.
        var rec = devices.Single(d => d.method_type == MfaMethodType.RecoveryCode);
        foreach (var h in hashes) rec.Metadata.Should().Contain(h);
        foreach (var raw in rawCodes) rec.Metadata.Should().NotContain(raw);
    }

    [Fact] // TC-INT-04 — BR-AUTH-05: a privileged user passes the MFA gate only after enrolling+verifying
    public async Task Privileged_user_clears_the_mfa_gate_after_verifying()
    {
        var (tenantId, userId) = await SeedUserAsync(privileged: true);

        await using (var ctx = _fx.CreateContext())
        {
            var auth = new AuthRepository(ctx, scopeFactory: null!);   // RequiresMfaSetupAsync ignores the scope factory
            (await auth.RequiresMfaSetupAsync(userId, tenantId)).Should().BeTrue();   // blocked at login
        }

        var secret = _totp.GenerateSecret();
        var deviceId = await EnrollAsync(tenantId, userId, _vault.Store(secret));
        await VerifyAsync(deviceId, userId, tenantId, _recovery.Generate(AppDefaultsRecoveryCount).Select(_recovery.Hash).ToList());

        await using (var ctx = _fx.CreateContext())
        {
            var auth = new AuthRepository(ctx, scopeFactory: null!);
            (await auth.RequiresMfaSetupAsync(userId, tenantId)).Should().BeFalse();   // no more 403 MFA_REQUIRED
        }
    }

    [Fact] // TC-INT-06 — re-enroll supersedes: prior TOTP revoked, exactly one is_primary (partial UQ holds)
    public async Task Re_enrolling_supersedes_the_prior_totp_leaving_one_primary()
    {
        var (tenantId, userId) = await SeedUserAsync();

        var first = await EnrollAndVerifyAsync(tenantId, userId);
        var second = await EnrollAndVerifyAsync(tenantId, userId);

        await using var ctx = _fx.CreateContext();
        var totps = await ctx.MfaDevices.AsNoTracking()
            .Where(m => m.UserId == userId && m.method_type == MfaMethodType.Totp)
            .ToListAsync();

        totps.Count(d => d.IsPrimary).Should().Be(1);
        totps.Single(d => d.IsPrimary).MfaDeviceId.Should().Be(second);
        totps.Single(d => d.MfaDeviceId == first).RevokedAt.Should().NotBeNull();
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private const int AppDefaultsRecoveryCount = 10;

    private async Task<Guid> EnrollAndVerifyAsync(Guid tenantId, Guid userId)
    {
        var secret = _totp.GenerateSecret();
        var deviceId = await EnrollAsync(tenantId, userId, _vault.Store(secret));
        await VerifyAsync(deviceId, userId, tenantId, _recovery.Generate(AppDefaultsRecoveryCount).Select(_recovery.Hash).ToList());
        return deviceId;
    }

    private async Task<Guid> EnrollAsync(Guid tenantId, Guid userId, string secretRef)
    {
        await using var ctx = _fx.CreateContext();
        var repo = new MfaRepository(ctx);
        var id = await repo.AddPendingTotpAsync(
            new MfaTotpEnrollment(tenantId, userId, secretRef, "My Phone", "totp_" + Guid.NewGuid().ToString("N")[..12]));
        await ctx.SaveChangesAsync();
        return id;
    }

    private async Task VerifyAsync(Guid deviceId, Guid userId, Guid tenantId, IReadOnlyList<string> hashes)
    {
        // Both the TOTP UPDATE and the recovery_code INSERT commit in ONE SaveChanges — exercises the
        // scaffolded 1:1 User↔MfaDevice config with several device rows for one user.
        await using var ctx = _fx.CreateContext();
        var repo = new MfaRepository(ctx);
        await repo.ActivateAndSupersedeAsync(deviceId, userId, DateTime.UtcNow);
        await repo.AddRecoveryCodesAsync(userId, tenantId, hashes);
        await ctx.SaveChangesAsync();
    }

    private async Task<(Guid tenantId, Guid userId)> SeedUserAsync(bool privileged = false)
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var suffix = Guid.NewGuid().ToString("N")[..10];

        await using var conn = await _fx.DataSource.OpenConnectionAsync();

        await ExecAsync(conn,
            "INSERT INTO core.tenants (tenant_id, tenant_code, tenant_name) VALUES (@t, @code, 'Test Tenant')",
            ("t", tenantId), ("code", "tc_" + suffix));

        await ExecAsync(conn,
            "INSERT INTO core.users (user_id, tenant_id, user_code, email, password_hash, first_name, last_name) " +
            "VALUES (@u, @t, @uc, @em::citext, 'x', 'Test', 'User')",
            ("u", userId), ("t", tenantId), ("uc", "uc_" + suffix), ("em", suffix + "@test.io"));

        if (privileged)
        {
            var roleId = Guid.NewGuid();
            await ExecAsync(conn,
                "INSERT INTO core.roles (role_id, tenant_id, role_code, role_name, is_active) VALUES (@r, @t, @rc, 'Company Admin', true)",
                ("r", roleId), ("t", tenantId), ("rc", RolePermissionMap.RoleCodes.TenantAdmin));
            await ExecAsync(conn,
                "INSERT INTO core.user_roles (user_id, role_id) VALUES (@u, @r)",
                ("u", userId), ("r", roleId));
        }

        return (tenantId, userId);
    }

    private static async Task ExecAsync(NpgsqlConnection conn, string sql, params (string Name, object Value)[] parameters)
    {
        await using var cmd = new NpgsqlCommand(sql, conn);
        foreach (var (name, value) in parameters)
            cmd.Parameters.AddWithValue(name, value);
        await cmd.ExecuteNonQueryAsync();
    }
}
