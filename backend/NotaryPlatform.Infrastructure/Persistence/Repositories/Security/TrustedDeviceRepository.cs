using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Shared.Constants;
using NotaryPlatform.Domain.Features.Security.Enums;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Security;

/// <summary>
/// EF Core implementation of the use-case <see cref="ITrustedDeviceRepository"/> over
/// <c>security.trusted_devices</c> (UC-AUTH-08). Reads are <c>AsNoTracking</c>; writes are tracked so
/// <c>TransactionBehavior</c>'s commit flushes them. Devices are mutated only through the
/// <see cref="TrustedDevice"/> behavior partial (§7.5). Every query/write is scoped to (user, tenant).
/// Replaces the deleted, non-functional domain-aggregate repository (decision O-1).
/// </summary>
public sealed class TrustedDeviceRepository : ITrustedDeviceRepository
{
    private readonly NotaryPlatformDbContext _context;

    public TrustedDeviceRepository(NotaryPlatformDbContext context) => _context = context;

    public Task<bool> IsDeviceTrustedAsync(Guid userId, Guid tenantId, string fingerprint, DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fingerprint))
            return Task.FromResult(false);

        // BR-AUTH-08: trust is valid only within TrustedDevicePeriod of when it was granted.
        var cutoff = nowUtc - AppDefaults.Security.TrustedDevicePeriod;

        return _context.TrustedDevices
            .AsNoTracking()
            .AnyAsync(d => d.UserId == userId
                           && d.TenantId == tenantId
                           && d.DeviceFingerprint == fingerprint
                           && d.status == DeviceStatus.Trusted
                           && d.RevokedAt == null
                           && d.DeletedAt == null
                           && d.TrustedAt != null
                           && d.TrustedAt > cutoff,
                cancellationToken);
    }

    public async Task<TrustDeviceOutcome> TrustDeviceAsync(TrustedDeviceRegistration registration, DateTime whenUtc, CancellationToken cancellationToken = default)
    {
        // The fingerprint is UNIQUE per tenant (uq_trusted_devices_fingerprint), so at most one row can match.
        var existing = await _context.TrustedDevices
            .FirstOrDefaultAsync(d => d.TenantId == registration.TenantId
                                      && d.DeviceFingerprint == registration.Fingerprint
                                      && d.DeletedAt == null,
                cancellationToken);

        if (existing is not null)
        {
            // O-5: never let a user take over a fingerprint already owned by someone else in the tenant.
            if (existing.UserId != registration.UserId)
                return TrustDeviceOutcome.ConflictDifferentUser;

            existing.RefreshHints(registration.DeviceName, registration.Platform, registration.Browser, registration.Ip);
            existing.Trust(whenUtc);   // re-trust — restarts the BR-AUTH-08 window
            return TrustDeviceOutcome.Trusted;
        }

        var deviceCode = "td_" + Guid.NewGuid().ToString("N")[..12];   // unique per tenant (<= 50 chars)
        var device = TrustedDevice.TrustNew(
            registration.TenantId,
            registration.UserId,
            deviceCode,
            registration.Fingerprint,
            registration.DeviceName,
            registration.Platform,
            registration.Browser,
            registration.Ip,
            whenUtc);

        await _context.TrustedDevices.AddAsync(device, cancellationToken);
        return TrustDeviceOutcome.Trusted;
    }

    public async Task StampSeenAsync(Guid userId, Guid tenantId, string fingerprint, DateTime whenUtc, CancellationToken cancellationToken = default)
    {
        var device = await _context.TrustedDevices
            .FirstOrDefaultAsync(d => d.UserId == userId
                                      && d.TenantId == tenantId
                                      && d.DeviceFingerprint == fingerprint
                                      && d.status == DeviceStatus.Trusted
                                      && d.RevokedAt == null
                                      && d.DeletedAt == null,
                cancellationToken);
        device?.StampSeen(whenUtc);
    }

    public async Task<IReadOnlyList<TrustedDeviceRecord>> ListAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.TrustedDevices
            .AsNoTracking()
            .Where(d => d.UserId == userId && d.TenantId == tenantId && d.DeletedAt == null)
            .OrderByDescending(d => d.TrustedAt ?? d.FirstSeenAt)
            .ToListAsync(cancellationToken);

        var period = AppDefaults.Security.TrustedDevicePeriod;

        return rows
            .Select(d => new TrustedDeviceRecord(
                d.TrustedDeviceId,
                d.DeviceName,
                d.Platform,
                d.TrustedAt,
                d.LastSeenAt,
                d.TrustedAt is { } t ? t + period : null,   // computed expiry (no trusted_until_utc column)
                d.status.ToString().ToLowerInvariant()))
            .ToList();
    }

    public async Task<bool> RevokeAsync(Guid trustedDeviceId, Guid userId, Guid tenantId, DateTime whenUtc, CancellationToken cancellationToken = default)
    {
        // Ownership guard: only the caller's own, non-deleted device. Not found ⇒ false ⇒ 404 (anti-enumeration).
        var device = await _context.TrustedDevices
            .FirstOrDefaultAsync(d => d.TrustedDeviceId == trustedDeviceId
                                      && d.UserId == userId
                                      && d.TenantId == tenantId
                                      && d.DeletedAt == null,
                cancellationToken);
        if (device is null)
            return false;

        device.Revoke(whenUtc);   // idempotent — a second revoke just re-stamps
        return true;
    }
}
