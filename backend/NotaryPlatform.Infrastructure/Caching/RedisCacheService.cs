using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using NotaryPlatform.Application.Abstractions.Caching;
using NotaryPlatform.Application.Shared.Interfaces;
using StackExchange.Redis;

namespace NotaryPlatform.Infrastructure.Caching;

/// <summary>
/// Redis implementation of <see cref="ICacheService"/>.
/// Wraps <see cref="IDistributedCache"/> (for standard get/set operations with JSON
/// serialization) and <see cref="IConnectionMultiplexer"/> (for prefix-based key
/// scanning required by <see cref="RemoveByPrefixAsync"/>).
///
/// LEARNING — Why two Redis abstractions?
///   <see cref="IDistributedCache"/> is the standard ASP.NET Core cache interface.
///   It supports get, set, and remove by exact key — nothing more.
///   <see cref="IConnectionMultiplexer"/> gives direct access to the Redis SCAN
///   command, which we need to delete all keys matching a prefix pattern.
///   Both use the same underlying Redis connection pool; there is no extra overhead.
///
/// LEARNING — JSON serialization choice:
///   System.Text.Json is used (not Newtonsoft) for speed and zero-dependency reasons.
///   All cached types must be JSON-serializable; value objects with only init
///   properties need a no-arg constructor or a custom converter.
///
/// LEARNING — SCAN vs KEYS:
///   Redis KEYS is O(N) and blocks the server. SCAN is iterative and non-blocking.
///   StackExchange.Redis's <c>IServer.KeysAsync</c> uses SCAN internally.
///   For Redis Cluster, SCAN must be executed per shard — this implementation
///   targets single-node or primary/replica setups.
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;
    private readonly TimeSpan _defaultExpiry;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public RedisCacheService(
        IDistributedCache cache,
        IConnectionMultiplexer redis,
        IOptions<CacheSettings> settings)
    {
        _cache = cache;
        _redis = redis;
        _defaultExpiry = TimeSpan.FromMinutes(settings.Value.DefaultExpiryMinutes);
    }

    // ── Read ──────────────────────────────────────────────────────────────────────

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var bytes = await _cache.GetAsync(key, cancellationToken);
        if (bytes is null || bytes.Length == 0) return default;

        return JsonSerializer.Deserialize<T>(bytes, SerializerOptions);
    }

    // ── Write ─────────────────────────────────────────────────────────────────────

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, SerializerOptions);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpiry ?? _defaultExpiry,
        };

        await _cache.SetAsync(key, bytes, options, cancellationToken);
    }

    // ── Delete ────────────────────────────────────────────────────────────────────

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => _cache.RemoveAsync(key, cancellationToken);

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // SCAN across the Redis keyspace for all keys matching the prefix pattern.
        // We target the first (primary) endpoint; extend to all endpoints for Cluster.
        var endpoint = _redis.GetEndPoints().FirstOrDefault()
            ?? throw new InvalidOperationException("No Redis endpoints are configured.");

        var server = _redis.GetServer(endpoint);
        var db = _redis.GetDatabase();

        var keys = new List<RedisKey>();
        await foreach (var key in server.KeysAsync(pattern: $"{prefix}*").WithCancellation(cancellationToken))
            keys.Add(key);

        if (keys.Count == 0) return;

        // Batch delete — one round-trip instead of N.
        await db.KeyDeleteAsync(keys.ToArray());
    }

    // ── Cache-aside ───────────────────────────────────────────────────────────────

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default)
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached is not null) return cached;

        var value = await factory(cancellationToken);
        await SetAsync(key, value, absoluteExpiry, cancellationToken);
        return value;
    }
}
