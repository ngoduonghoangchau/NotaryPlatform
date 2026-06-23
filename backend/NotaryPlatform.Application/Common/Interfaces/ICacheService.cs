namespace NotaryPlatform.Application.Common.Interfaces;

/// <summary>
/// Application-level cache abstraction. Implemented by Infrastructure.Caching.RedisCacheService.
/// Designed for query-result caching — not for session or auth token storage.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Returns the cached value for <paramref name="key"/>, or null if absent / expired.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores <paramref name="value"/> under <paramref name="key"/> for the specified TTL.
    /// A null <paramref name="absoluteExpiry"/> uses the configured default TTL.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a single cache entry. No-op if the key does not exist.
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all entries whose keys start with <paramref name="prefix"/>.
    /// Used to invalidate a feature's entire cache on write operations.
    /// </summary>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cache-aside pattern helper: get from cache, or execute <paramref name="factory"/>
    /// and store the result before returning.
    /// </summary>
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default);
}
