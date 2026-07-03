namespace NotaryPlatform.Application.Abstractions.Authentication;

/// <summary>
/// Tracks failed login attempts and enforces the temporary lockout window (BR-AUTH-02).
///
/// LEARNING — why not store this on <c>core.users</c>?
///   The login handler runs inside <c>TransactionBehavior</c>; a failed login <c>throw</c>s,
///   which rolls back the EF transaction — so an EF-persisted failure counter would be rolled
///   back too and never accumulate. This tracker is backed by Redis (via <c>ICacheService</c>),
///   which lives outside the EF transaction, so failures survive the rollback.
/// </summary>
public interface ILoginAttemptTracker
{
    /// <summary>
    /// Returns the UTC instant the lockout lifts if the (tenant, email) is currently locked,
    /// otherwise null. The lock auto-expires via the cache entry's TTL (BR-AUTH-02 auto-lift).
    /// </summary>
    Task<DateTimeOffset?> GetLockoutExpiryAsync(Guid tenantId, string email, CancellationToken cancellationToken = default);

    /// <summary>Records one failed attempt; locks the account once the max is reached.</summary>
    Task RegisterFailureAsync(Guid tenantId, string email, CancellationToken cancellationToken = default);

    /// <summary>Clears the failure counter after a successful authentication.</summary>
    Task ResetAsync(Guid tenantId, string email, CancellationToken cancellationToken = default);
}
