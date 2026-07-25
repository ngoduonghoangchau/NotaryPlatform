namespace NotaryPlatform.Application.Abstractions.Authentication;

/// <summary>
/// Use-case-scoped data-access port for <c>security.mfa_devices</c> (UC-AUTH-06, D-4). Like
/// <see cref="IAuthRepository"/>, it reads/writes the scaffolded EF entity directly (MFA has no rich
/// domain aggregate) and mutates the entity only through its behavior partial (§7.5). Writes are tracked
/// and committed by <c>TransactionBehavior</c> — no <c>SaveChanges</c> here. Every operation is scoped to
/// the calling user (+ tenant) so a caller can never touch another user's devices.
/// </summary>
public interface IMfaRepository
{
    /// <summary>
    /// Inserts a new <b>pending</b> TOTP device and returns its id. Any prior <i>pending</i> (unverified,
    /// non-revoked) TOTP enrollment for the user is expired first — one in-flight enrollment at a time.
    /// </summary>
    Task<Guid> AddPendingTotpAsync(MfaTotpEnrollment enrollment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a non-revoked TOTP device by id, scoped to (user, tenant), or null if none matches (⇒ 404).
    /// Returns the device <b>regardless of verification state</b> (with <see cref="PendingMfaDeviceRecord.IsVerified"/>)
    /// so the handler can distinguish an already-verified device (⇒ 409) from a pending one.
    /// </summary>
    Task<PendingMfaDeviceRecord?> FindPendingByIdAsync(Guid mfaDeviceId, Guid userId, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the device verified + primary (<c>is_verified</c>, <c>verified_at</c>, <c>status='trusted'</c>,
    /// <c>is_primary=true</c>) and revokes any <b>prior verified TOTP</b> device for the user so only one
    /// stays primary (D-5, honoring the one-primary partial unique index).
    /// </summary>
    Task ActivateAndSupersedeAsync(Guid mfaDeviceId, Guid userId, DateTime whenUtc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a fresh set of recovery-code <b>hashes</b> (never the raw codes) as a single
    /// <c>recovery_code</c> device row's <c>metadata</c>, revoking any prior active set (D-3).
    /// </summary>
    Task AddRecoveryCodesAsync(Guid userId, Guid tenantId, IReadOnlyList<string> hashedCodes, CancellationToken cancellationToken = default);
}

/// <summary>Values required to persist a new pending TOTP enrollment (the vault reference, never the raw secret).</summary>
public sealed record MfaTotpEnrollment(
    Guid TenantId,
    Guid UserId,
    string SecretReference,
    string? Label,
    string DeviceCode);

/// <summary>Snapshot of an MFA device needed to verify it (id, its secret reference, and whether it is already verified).</summary>
public sealed record PendingMfaDeviceRecord(
    Guid MfaDeviceId,
    string? SecretReference,
    bool IsVerified);
