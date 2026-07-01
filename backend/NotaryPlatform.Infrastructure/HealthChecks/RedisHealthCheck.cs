using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace NotaryPlatform.Infrastructure.HealthChecks;

/// <summary>
/// Custom health check that verifies Redis responds to a PING and reports
/// round-trip latency.
///
/// LEARNING — why not use AddRedis() from AspNetCore.HealthChecks.Redis?
///   <c>AddRedis</c> accepts a raw connection string and creates its own
///   <see cref="IConnectionMultiplexer"/>. This custom check reuses the
///   singleton <see cref="IConnectionMultiplexer"/> that the rest of the app
///   (cache, SignalR backplane) already holds, avoiding a second Redis connection.
///
/// LEARNING — IDatabase.PingAsync:
///   Sends a Redis PING to the server and measures the round-trip time.
///   This is the lightest possible operation — it validates connectivity and
///   latency without touching any keys.
/// </summary>
internal sealed class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;

    public RedisHealthCheck(IConnectionMultiplexer redis) => _redis = redis;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var db      = _redis.GetDatabase();
            var latency = await db.PingAsync();

            var data = new Dictionary<string, object>
            {
                ["latency_ms"]   = latency.TotalMilliseconds,
                ["is_connected"] = _redis.IsConnected,
            };

            // Flag as Degraded when latency exceeds 100 ms — not unhealthy,
            // but worth alerting on in production.
            return latency.TotalMilliseconds < 100
                ? HealthCheckResult.Healthy("Redis is reachable.", data)
                : HealthCheckResult.Degraded($"Redis latency is elevated ({latency.TotalMilliseconds:F0} ms).", data: data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis health check threw an exception.", ex);
        }
    }
}
