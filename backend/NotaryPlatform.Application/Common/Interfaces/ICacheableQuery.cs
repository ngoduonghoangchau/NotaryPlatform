namespace NotaryPlatform.Application.Common.Interfaces;

/// <summary>
/// Extends <see cref="IQuery{TResponse}"/> to opt a query into the
/// CachingBehavior pipeline. Implement this interface on any query whose
/// result should be cached in Redis.
/// </summary>
public interface ICacheableQuery<out TResponse> : IQuery<TResponse>
{
    /// <summary>
    /// Unique cache key for this query. Must encode all parameters that affect the result.
    /// Recommended format: "Feature:EntityType:param1:param2"
    /// Example: "Identity:NotaryProfile:tid_abc:usr_xyz"
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// How long the cached result remains valid.
    /// Null → use the system default from AppDefaults.Cache.DefaultExpiry.
    /// </summary>
    TimeSpan? CacheExpiry => null;

    /// <summary>
    /// When true, forces a cache refresh even if a valid entry exists.
    /// Useful for "reload" actions triggered by the user.
    /// </summary>
    bool ForceRefresh => false;
}
