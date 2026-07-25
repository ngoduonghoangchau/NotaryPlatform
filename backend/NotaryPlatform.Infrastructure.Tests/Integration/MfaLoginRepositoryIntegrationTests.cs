using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Domain.Features.Security.Enums;
using NotaryPlatform.Infrastructure.Persistence.Repositories.Security;
using NotaryPlatform.Infrastructure.Services.Authentication;
using Xunit;

namespace NotaryPlatform.Infrastructure.Tests.Integration;

/// <summary>
/// UC-AUTH-07 DB-bound truths against a real PostgreSQL container: the login discriminator
/// (<c>HasActiveMfaAsync</c>), the challenge-time TOTP lookup, and the single-use recovery-code consume
/// (usedAt persisted in the metadata jsonb; replay fails) — the parts unit tests (mocked repo) can't prove.
/// </summary>
[Collection(PostgresCollection.Name)]
public sealed class MfaLoginRepositoryIntegrationTests
{
    private readonly PostgresSchemaFixture _fx;
    private readonly RecoveryCodeService _recovery = new();

    private const string SecretRef = "cipher-ref-placeholder";

    public MfaLoginRepositoryIntegrationTests(PostgresSchemaFixture fx) => _fx = fx;

    [Fact] // TC-INT (login discriminator) — no MFA before enrol, MFA after enrol+verify; FindActiveTotp resolves the device
    public async Task HasActiveMfa_and_FindActiveTotp_reflect_a_verified_device()
    {
        var (tenantId, userId) = await SeedUserAsync();

        await using (var ctx = _fx.CreateContext())
        {
            var repo = new MfaRepository(ctx);
            (await repo.HasActiveMfaAsync(userId, tenantId)).Should().BeFalse();   // nothing enrolled yet
        }

        var deviceId = await EnrollAndVerifyTotpAsync(tenantId, userId);

        await using (var read = _fx.CreateContext())
        {
            var repo = new MfaRepository(read);
            (await repo.HasActiveMfaAsync(userId, tenantId)).Should().BeTrue();

            var totp = await repo.FindActiveTotpAsync(userId, tenantId);
            totp.Should().NotBeNull();
            totp!.MfaDeviceId.Should().Be(deviceId);
            totp.SecretReference.Should().Be(SecretRef);
        }
    }

    [Fact] // TC-INT-05 — recovery codes are single-use: consume marks usedAt, replay fails, siblings stay usable
    public async Task Recovery_code_is_single_use()
    {
        var (tenantId, userId) = await SeedUserAsync();

        var codeA = "aaaa-bbbb";
        var codeB = "cccc-dddd";
        await using (var ctx = _fx.CreateContext())
        {
            var repo = new MfaRepository(ctx);
            await repo.AddRecoveryCodesAsync(userId, tenantId, [_recovery.Hash(codeA), _recovery.Hash(codeB)]);
            await ctx.SaveChangesAsync();
        }

        // Consume A once.
        await using (var ctx = _fx.CreateContext())
        {
            var repo = new MfaRepository(ctx);
            (await repo.TryConsumeRecoveryCodeAsync(userId, tenantId, _recovery.Hash(codeA), DateTime.UtcNow)).Should().BeTrue();
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = _fx.CreateContext())
        {
            var repo = new MfaRepository(ctx);
            // Replay of A now fails (single-use)…
            (await repo.TryConsumeRecoveryCodeAsync(userId, tenantId, _recovery.Hash(codeA), DateTime.UtcNow)).Should().BeFalse();
            // …a code that was never issued fails…
            (await repo.TryConsumeRecoveryCodeAsync(userId, tenantId, _recovery.Hash("zzzz-zzzz"), DateTime.UtcNow)).Should().BeFalse();
            // …but sibling B is still usable.
            (await repo.TryConsumeRecoveryCodeAsync(userId, tenantId, _recovery.Hash(codeB), DateTime.UtcNow)).Should().BeTrue();
            await ctx.SaveChangesAsync();
        }

        // The persisted metadata records usedAt for the consumed codes; the raw codes are never stored.
        await using (var read = _fx.CreateContext())
        {
            var recovery = await read.MfaDevices.AsNoTracking()
                .FirstAsync(m => m.UserId == userId && m.method_type == MfaMethodType.RecoveryCode);
            recovery.Metadata.Should().Contain("usedAt").And.Contain(_recovery.Hash(codeA));
            recovery.Metadata.Should().NotContain(codeA).And.NotContain(codeB);
        }
    }

    [Fact] // audit — a successful challenge stamps last_used_at
    public async Task StampDeviceUsed_sets_last_used_at()
    {
        var (tenantId, userId) = await SeedUserAsync();
        var deviceId = await EnrollAndVerifyTotpAsync(tenantId, userId);
        var when = new DateTime(2026, 7, 25, 8, 30, 0, DateTimeKind.Utc);

        await using (var ctx = _fx.CreateContext())
        {
            var repo = new MfaRepository(ctx);
            await repo.StampDeviceUsedAsync(deviceId, when, CancellationToken.None);
            await ctx.SaveChangesAsync();
        }

        await using (var read = _fx.CreateContext())
        {
            var device = await read.MfaDevices.AsNoTracking().FirstAsync(m => m.MfaDeviceId == deviceId);
            device.LastUsedAt.Should().BeCloseTo(when, TimeSpan.FromSeconds(1));
        }
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private async Task<Guid> EnrollAndVerifyTotpAsync(Guid tenantId, Guid userId)
    {
        Guid deviceId;
        await using (var ctx = _fx.CreateContext())
        {
            var repo = new MfaRepository(ctx);
            deviceId = await repo.AddPendingTotpAsync(
                new MfaTotpEnrollment(tenantId, userId, SecretRef, "My Phone", "totp_" + Guid.NewGuid().ToString("N")[..12]));
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = _fx.CreateContext())
        {
            var repo = new MfaRepository(ctx);
            await repo.ActivateAndSupersedeAsync(deviceId, userId, DateTime.UtcNow);
            await ctx.SaveChangesAsync();
        }

        return deviceId;
    }

    private async Task<(Guid tenantId, Guid userId)> SeedUserAsync()
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
