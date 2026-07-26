using System.Net;

namespace NotaryPlatform.Application.Abstractions.Authentication;

/// <summary>
/// Use-case-scoped data-access port for <c>security.trusted_devices</c> (UC-AUTH-08). Like
/// <see cref="IMfaRepository"/> / <see cref="IAuthRepository"/>, it reads/writes the scaffolded EF entity
/// directly (no domain aggregate — the pre-existing one was dead code, removed per decision O-1) and mutates
/// it only through its behavior partial (§7.5). Writes are tracked and committed by
/// <c>TransactionBehavior</c> — no <c>SaveChanges</c> here. Every operation is scoped to the calling user
/// (+ tenant) so a caller can never see or touch another user's trusted devices (S-1 user+tenant binding).
/// </summary>
public interface ITrustedDeviceRepository
{
    /// <summary>
    /// True when the user has a <b>trusted, non-revoked, non-deleted</b> device for <paramref name="fingerprint"/>
    /// whose trust is still within <c>AppDefaults.Security.TrustedDevicePeriod</c> of
    /// <paramref name="nowUtc"/> (BR-AUTH-08). Scoped to (user, tenant). Drives the login MFA bypass.
    /// </summary>
    Task<bool> IsDeviceTrustedAsync(Guid userId, Guid tenantId, string fingerprint, DateTime nowUtc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers/renews trust for a device after a proven MFA verification. Upserts on the per-tenant-unique
    /// fingerprint: creates a new <c>trusted</c> row, or re-trusts the caller's existing row. If the fingerprint
    /// is already owned by a <b>different</b> user in the tenant, returns
    /// <see cref="TrustDeviceOutcome.ConflictDifferentUser"/> and writes nothing (decision O-5 — no hijacking).
    /// Tracked write.
    /// </summary>
    Task<TrustDeviceOutcome> TrustDeviceAsync(TrustedDeviceRegistration registration, DateTime whenUtc, CancellationToken cancellationToken = default);

    /// <summary>Stamps <c>last_seen_at</c> on the caller's trusted device for <paramref name="fingerprint"/> (best-effort, on a bypass). Tracked write.</summary>
    Task StampSeenAsync(Guid userId, Guid tenantId, string fingerprint, DateTime whenUtc, CancellationToken cancellationToken = default);

    /// <summary>Lists the caller's non-deleted trusted devices (with the computed expiry) for the management UI.</summary>
    Task<IReadOnlyList<TrustedDeviceRecord>> ListAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-revokes the caller's device by id (<c>status=revoked</c>, <c>revoked_at=now</c>). Ownership guard:
    /// returns false when no non-deleted device with that id belongs to (user, tenant) — the handler maps that
    /// to a 404. Idempotent for an already-revoked device. Tracked write.
    /// </summary>
    Task<bool> RevokeAsync(Guid trustedDeviceId, Guid userId, Guid tenantId, DateTime whenUtc, CancellationToken cancellationToken = default);
}

/// <summary>Outcome of a trust upsert — distinguishes success from the O-5 cross-user fingerprint conflict.</summary>
public enum TrustDeviceOutcome
{
    /// <summary>The device was created or re-trusted for the caller.</summary>
    Trusted,

    /// <summary>The fingerprint is already trusted by a different user in the tenant — rejected (⇒ 409).</summary>
    ConflictDifferentUser,
}

/// <summary>Values required to register/renew a trusted device (client-supplied fingerprint + optional hints).</summary>
public sealed record TrustedDeviceRegistration(
    Guid TenantId,
    Guid UserId,
    string Fingerprint,
    string? DeviceName,
    string? Platform,
    string? Browser,
    IPAddress? Ip);

/// <summary>Read model for one of the caller's trusted devices (for list/management).</summary>
public sealed record TrustedDeviceRecord(
    Guid TrustedDeviceId,
    string? DeviceName,
    string? Platform,
    DateTime? TrustedAt,
    DateTime? LastSeenAt,
    DateTime? ExpiresAtUtc,
    string Status);
