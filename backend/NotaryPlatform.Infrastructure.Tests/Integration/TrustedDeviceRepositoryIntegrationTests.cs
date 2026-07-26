using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Infrastructure.Persistence.Repositories.Security;
using Xunit;

namespace NotaryPlatform.Infrastructure.Tests.Integration;

/// <summary>
/// UC-AUTH-08 DB-bound truths against a real PostgreSQL container (TC-DB-01…05): the trust→bypass→expire→revoke
/// round trip, the per-tenant-unique fingerprint (upsert + cross-user O-5 conflict), unique device codes, the
/// updated-at trigger, and user/tenant scoping — the parts unit tests (mocked repo) can't prove.
/// </summary>
[Collection(PostgresCollection.Name)]
public sealed class TrustedDeviceRepositoryIntegrationTests
{
    private readonly PostgresSchemaFixture _fx;
    private static readonly DateTime T0 = new(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc);

    public TrustedDeviceRepositoryIntegrationTests(PostgresSchemaFixture fx) => _fx = fx;

    [Fact] // TC-DB-01 — trust → bypass (within window) → expire (past window) → revoke (no bypass)
    public async Task Trust_bypass_expire_revoke_round_trip()
    {
        var (tenantId, userId) = await SeedUserAsync();
        const string fp = "fp-roundtrip-123456";

        await using (var ctx = _fx.CreateContext())
        {
            var repo = new TrustedDeviceRepository(ctx);
            (await repo.TrustDeviceAsync(Reg(tenantId, userId, fp), T0)).Should().Be(TrustDeviceOutcome.Trusted);
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = _fx.CreateContext())
        {
            var repo = new TrustedDeviceRepository(ctx);
            (await repo.IsDeviceTrustedAsync(userId, tenantId, fp, T0.AddDays(29))).Should().BeTrue();    // within 30 days
            (await repo.IsDeviceTrustedAsync(userId, tenantId, fp, T0.AddDays(31))).Should().BeFalse();   // BR-AUTH-08 expired
        }

        var deviceId = await SingleDeviceIdAsync(userId);

        await using (var ctx = _fx.CreateContext())
        {
            var repo = new TrustedDeviceRepository(ctx);
            (await repo.RevokeAsync(deviceId, userId, tenantId, T0.AddDays(1))).Should().BeTrue();
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = _fx.CreateContext())
        {
            var repo = new TrustedDeviceRepository(ctx);
            (await repo.IsDeviceTrustedAsync(userId, tenantId, fp, T0.AddDays(2))).Should().BeFalse();   // revoked ⇒ no bypass
        }
    }

    [Fact] // TC-DB-02 — fingerprint is unique per tenant: same user re-trusts (upsert), different user conflicts (O-5)
    public async Task Fingerprint_unique_per_tenant_upserts_and_rejects_cross_user()
    {
        var (tenantId, userId) = await SeedUserAsync();
        var (_, otherUserId) = await SeedUserAsync(tenantId);   // second user in the SAME tenant
        const string fp = "fp-shared-123456";

        await TrustAsync(tenantId, userId, fp, T0);
        await TrustAsync(tenantId, userId, fp, T0.AddDays(1));   // re-trust ⇒ upsert, not a duplicate

        await using (var read = _fx.CreateContext())
        {
            (await read.TrustedDevices.AsNoTracking()
                .CountAsync(d => d.TenantId == tenantId && d.DeviceFingerprint == fp)).Should().Be(1);
        }

        // A different user in the tenant cannot take the fingerprint (O-5) — and nothing is written.
        await using (var ctx = _fx.CreateContext())
        {
            var repo = new TrustedDeviceRepository(ctx);
            (await repo.TrustDeviceAsync(Reg(tenantId, otherUserId, fp), T0)).Should().Be(TrustDeviceOutcome.ConflictDifferentUser);
            await ctx.SaveChangesAsync();
        }

        await using (var read = _fx.CreateContext())
        {
            var row = await read.TrustedDevices.AsNoTracking().SingleAsync(d => d.TenantId == tenantId && d.DeviceFingerprint == fp);
            row.UserId.Should().Be(userId);   // still owned by the original user
        }
    }

    [Fact] // TC-DB-03 — multiple devices for one user get unique device codes (uq_trusted_devices_tenant_code holds)
    public async Task Multiple_devices_get_unique_device_codes()
    {
        var (tenantId, userId) = await SeedUserAsync();

        await TrustAsync(tenantId, userId, "fp-device-one-1", T0);
        await TrustAsync(tenantId, userId, "fp-device-two-2", T0);   // must not violate the per-tenant code UQ

        await using var read = _fx.CreateContext();
        var codes = await read.TrustedDevices.AsNoTracking()
            .Where(d => d.UserId == userId).Select(d => d.DeviceCode).ToListAsync();

        codes.Should().HaveCount(2).And.OnlyHaveUniqueItems();
    }

    [Fact] // TC-DB-04 — the updated_at trigger bumps updated_at on a revoke (EF never sets it)
    public async Task Updated_at_trigger_fires_on_revoke()
    {
        var (tenantId, userId) = await SeedUserAsync();
        await TrustAsync(tenantId, userId, "fp-trigger-123456", T0);

        Guid deviceId;
        DateTime updatedAfterInsert;
        await using (var read = _fx.CreateContext())
        {
            var d = await read.TrustedDevices.AsNoTracking().FirstAsync(x => x.UserId == userId);
            deviceId = d.TrustedDeviceId;
            updatedAfterInsert = d.UpdatedAt;
        }

        await using (var ctx = _fx.CreateContext())
        {
            var repo = new TrustedDeviceRepository(ctx);
            await repo.RevokeAsync(deviceId, userId, tenantId, T0.AddDays(1));
            await ctx.SaveChangesAsync();
        }

        await using var read2 = _fx.CreateContext();
        var revoked = await read2.TrustedDevices.AsNoTracking().FirstAsync(x => x.TrustedDeviceId == deviceId);
        revoked.RevokedAt.Should().NotBeNull();
        revoked.UpdatedAt.Should().BeAfter(updatedAfterInsert);   // trigger maintained it
    }

    [Fact] // TC-DB-05 — trusted-device lookup + list are scoped by (user, tenant); no cross-user match
    public async Task Lookup_and_list_are_scoped_by_user_and_tenant()
    {
        var (tenantId, userId) = await SeedUserAsync();
        var (_, otherUserId) = await SeedUserAsync(tenantId);
        const string fp = "fp-scoped-123456";

        await TrustAsync(tenantId, userId, fp, T0);

        await using var ctx = _fx.CreateContext();
        var repo = new TrustedDeviceRepository(ctx);

        (await repo.IsDeviceTrustedAsync(userId, tenantId, fp, T0.AddDays(1))).Should().BeTrue();
        (await repo.IsDeviceTrustedAsync(otherUserId, tenantId, fp, T0.AddDays(1))).Should().BeFalse();   // different user
        (await repo.ListAsync(userId, tenantId)).Should().ContainSingle();
        (await repo.ListAsync(otherUserId, tenantId)).Should().BeEmpty();
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static TrustedDeviceRegistration Reg(Guid tenantId, Guid userId, string fingerprint) =>
        new(tenantId, userId, fingerprint, "Laptop", "Windows", "Chrome", Ip: null);

    private async Task TrustAsync(Guid tenantId, Guid userId, string fingerprint, DateTime whenUtc)
    {
        await using var ctx = _fx.CreateContext();
        var repo = new TrustedDeviceRepository(ctx);
        await repo.TrustDeviceAsync(Reg(tenantId, userId, fingerprint), whenUtc);
        await ctx.SaveChangesAsync();
    }

    private async Task<Guid> SingleDeviceIdAsync(Guid userId)
    {
        await using var read = _fx.CreateContext();
        var device = await read.TrustedDevices.AsNoTracking().FirstAsync(d => d.UserId == userId);
        return device.TrustedDeviceId;
    }

    private async Task<(Guid tenantId, Guid userId)> SeedUserAsync(Guid? tenantId = null)
    {
        var suffix = Guid.NewGuid().ToString("N")[..10];
        var tid = tenantId ?? Guid.NewGuid();
        var userId = Guid.NewGuid();

        await using var conn = await _fx.DataSource.OpenConnectionAsync();

        if (tenantId is null)
            await ExecAsync(conn,
                "INSERT INTO core.tenants (tenant_id, tenant_code, tenant_name) VALUES (@t, @code, 'Test Tenant')",
                ("t", tid), ("code", "tc_" + suffix));

        await ExecAsync(conn,
            "INSERT INTO core.users (user_id, tenant_id, user_code, email, password_hash, first_name, last_name) " +
            "VALUES (@u, @t, @uc, @em::citext, 'x', 'Test', 'User')",
            ("u", userId), ("t", tid), ("uc", "uc_" + suffix), ("em", suffix + "@test.io"));

        return (tid, userId);
    }

    private static async Task ExecAsync(NpgsqlConnection conn, string sql, params (string Name, object Value)[] parameters)
    {
        await using var cmd = new NpgsqlCommand(sql, conn);
        foreach (var (name, value) in parameters)
            cmd.Parameters.AddWithValue(name, value);
        await cmd.ExecuteNonQueryAsync();
    }
}
