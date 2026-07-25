using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Domain.Features.Security.Enums;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Security;

/// <summary>
/// EF Core implementation of <see cref="IMfaRepository"/> over <c>security.mfa_devices</c> (UC-AUTH-06).
/// Reads are <c>AsNoTracking</c>; writes are tracked so <c>TransactionBehavior</c>'s commit flushes them.
/// Devices are mutated only through the <see cref="MfaDevice"/> behavior partial (§7.5). Only scalar FKs
/// (<c>UserId</c>/<c>TenantId</c>) are set — the <c>User</c> navigation is never loaded, so the scaffolded
/// 1:1 <c>User↔MfaDevice</c> config does not object to a user holding several device rows (the table is a
/// registry — multiple methods per user).
/// </summary>
public sealed class MfaRepository : IMfaRepository
{
    private readonly NotaryPlatformDbContext _context;

    private static readonly JsonSerializerOptions MetadataJson = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public MfaRepository(NotaryPlatformDbContext context) => _context = context;

    public async Task<Guid> AddPendingTotpAsync(MfaTotpEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        // One in-flight enrollment: expire any prior pending (unverified, non-revoked) TOTP for this user.
        var stalePending = await _context.MfaDevices
            .Where(m => m.UserId == enrollment.UserId
                        && m.method_type == MfaMethodType.Totp
                        && !m.IsVerified
                        && m.RevokedAt == null
                        && m.DeletedAt == null)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        foreach (var stale in stalePending)
            stale.ExpirePending(now);

        var device = MfaDevice.EnrollTotp(
            enrollment.TenantId,
            enrollment.UserId,
            enrollment.DeviceCode,
            enrollment.Label,
            enrollment.SecretReference);

        await _context.MfaDevices.AddAsync(device, cancellationToken);
        return device.MfaDeviceId;   // client-generated (factory) — available before the commit
    }

    public async Task<PendingMfaDeviceRecord?> FindPendingByIdAsync(Guid mfaDeviceId, Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.MfaDevices
            .AsNoTracking()
            .Where(m => m.MfaDeviceId == mfaDeviceId
                        && m.UserId == userId
                        && m.TenantId == tenantId
                        && m.method_type == MfaMethodType.Totp
                        && m.RevokedAt == null
                        && m.DeletedAt == null)
            .Select(m => new PendingMfaDeviceRecord(m.MfaDeviceId, m.SecretReference, m.IsVerified))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task ActivateAndSupersedeAsync(Guid mfaDeviceId, Guid userId, DateTime whenUtc, CancellationToken cancellationToken = default)
    {
        // Demote/revoke any PRIOR verified TOTP so only one device stays primary (D-5).
        var priorVerified = await _context.MfaDevices
            .Where(m => m.UserId == userId
                        && m.method_type == MfaMethodType.Totp
                        && m.IsVerified
                        && m.RevokedAt == null
                        && m.DeletedAt == null
                        && m.MfaDeviceId != mfaDeviceId)
            .ToListAsync(cancellationToken);

        if (priorVerified.Count > 0)
        {
            foreach (var prior in priorVerified)
                prior.Revoke(whenUtc);

            // Flush the demotion as its OWN statement BEFORE the new device is promoted. The
            // `ux_mfa_devices_one_primary` partial unique index is checked per-statement, so demote and
            // promote must not land in the same batch (that momentarily leaves two is_primary=true rows
            // → 23505). This SaveChanges runs INSIDE TransactionBehavior's ambient transaction — it
            // flushes, it does not commit — so the supersede stays atomic.
            await _context.SaveChangesAsync(cancellationToken);
        }

        var device = await _context.MfaDevices
            .FirstOrDefaultAsync(m => m.MfaDeviceId == mfaDeviceId && m.UserId == userId, cancellationToken);
        device?.MarkVerifiedPrimary(whenUtc);
        // Promotion is flushed by TransactionBehavior's commit (or the caller's SaveChanges in tests).
    }

    public async Task AddRecoveryCodesAsync(Guid userId, Guid tenantId, IReadOnlyList<string> hashedCodes, CancellationToken cancellationToken = default)
    {
        // Supersede any prior active recovery-code set (a single active set per user).
        var priorSets = await _context.MfaDevices
            .Where(m => m.UserId == userId
                        && m.method_type == MfaMethodType.RecoveryCode
                        && m.RevokedAt == null
                        && m.DeletedAt == null)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        foreach (var prior in priorSets)
            prior.Revoke(now);

        var metadata = JsonSerializer.Serialize(
            new RecoveryCodesMetadata(hashedCodes.Select(h => new RecoveryCodeEntry(h, null)).ToArray()),
            MetadataJson);

        var deviceCode = "rec_" + Guid.NewGuid().ToString("N")[..12];
        var row = MfaDevice.CreateRecoveryCodeSet(tenantId, userId, deviceCode, metadata);
        await _context.MfaDevices.AddAsync(row, cancellationToken);
    }

    // Shape of the recovery_code row's metadata jsonb (D-3): { "recoveryCodes": [ { "hash": "...", "usedAt": null }, ... ] }
    private sealed record RecoveryCodesMetadata(RecoveryCodeEntry[] RecoveryCodes);

    private sealed record RecoveryCodeEntry(string Hash, DateTime? UsedAt);
}
