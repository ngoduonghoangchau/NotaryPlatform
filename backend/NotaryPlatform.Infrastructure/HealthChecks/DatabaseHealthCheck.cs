using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;

namespace NotaryPlatform.Infrastructure.HealthChecks;

/// <summary>
/// Custom health check that verifies PostgreSQL is reachable and able to
/// serve queries via EF Core's <see cref="M:CanConnectAsync"/>.
///
/// LEARNING — why not use AddNpgSql() from AspNetCore.HealthChecks.NpgSql?
///   <c>AddNpgSql</c> opens a raw ADO.NET connection, bypassing the EF Core
///   DbContext configuration (connection resiliency, interceptors, etc.).
///   This custom check goes through the same code path as the rest of the app,
///   so it catches EF configuration issues, not just TCP-level failures.
///
/// LEARNING — IServiceScopeFactory pattern for scoped dependencies:
///   Health checks are registered as singletons by the framework.
///   <see cref="NotaryPlatformDbContext"/> is scoped.
///   We create a fresh scope per check execution to safely resolve it.
/// </summary>
internal sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseHealthCheck(IServiceScopeFactory scopeFactory)
        => _scopeFactory = scopeFactory;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NotaryPlatformDbContext>();

            var sw = Stopwatch.StartNew();
            var canConnect = await db.Database.CanConnectAsync(cancellationToken);
            sw.Stop();

            if (!canConnect)
                return HealthCheckResult.Unhealthy("Database is not reachable.");

            return HealthCheckResult.Healthy(
                description: "Database is reachable.",
                data: new Dictionary<string, object>
                {
                    ["latency_ms"] = sw.ElapsedMilliseconds,
                });
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                description: "Database health check threw an exception.",
                exception: ex);
        }
    }
}
