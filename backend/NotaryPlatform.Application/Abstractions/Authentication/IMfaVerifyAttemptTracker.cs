namespace NotaryPlatform.Application.Abstractions.Authentication;

/// <summary>
/// Rate-limits MFA verification attempts per user (UC-AUTH-06 §5 hardening). A 6-digit code has only a
/// 10⁶ space, so the verify endpoint must be throttled to defeat brute force. Mirrors
/// <see cref="ILoginAttemptTracker"/>: Redis-backed so the counter lives OUTSIDE the EF transaction and
/// therefore survives the rollback that a failed (invalid-code) verify triggers.
/// </summary>
public interface IMfaVerifyAttemptTracker
{
    /// <summary>
    /// Returns the UTC instant the lockout lifts if the user is currently locked out of verification,
    /// otherwise null. The lock auto-expires via the cache entry's TTL.
    /// </summary>
    Task<DateTimeOffset?> GetLockoutExpiryAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Records one failed verification; locks the user out once the max is reached.</summary>
    Task RegisterFailureAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Clears the failure counter after a successful verification.</summary>
    Task ResetAsync(Guid userId, CancellationToken cancellationToken = default);
}
