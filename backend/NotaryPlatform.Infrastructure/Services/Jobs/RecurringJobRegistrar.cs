using Microsoft.Extensions.Logging;
using NotaryPlatform.Infrastructure.Persistence.Outbox;

namespace NotaryPlatform.Infrastructure.Services.Jobs;

/// <summary>
/// Registers all Hangfire recurring jobs at application startup.
/// Called from <c>DependencyInjection.AddInfrastructure</c> after the
/// Hangfire server is configured.
/// <para>
/// Add one private method per recurring job and invoke it from
/// <see cref="RegisterAll"/>. Each job gets a stable, human-readable
/// <c>jobId</c> that appears in the Hangfire dashboard.
/// </para>
/// </summary>
public sealed class RecurringJobRegistrar
{
    private readonly HangfireService _hangfire;
    private readonly ILogger<RecurringJobRegistrar> _logger;

    public RecurringJobRegistrar(HangfireService hangfire, ILogger<RecurringJobRegistrar> logger)
    {
        _hangfire = hangfire;
        _logger = logger;
    }

    /// <summary>
    /// Registers (or updates) every recurring job. Idempotent — safe to call
    /// on every application startup; Hangfire skips registration if the
    /// schedule has not changed.
    /// </summary>
    public void RegisterAll()
    {
        RegisterOutboxDispatcher();

        _logger.LogInformation("RecurringJobRegistrar: all recurring jobs registered");
    }

    // ── Job registrations ────────────────────────────────────────────────────

    private void RegisterOutboxDispatcher()
    {
        // Run every minute. For sub-minute polling without Hangfire Pro,
        // the job re-enqueues itself via JobScheduler.TriggerOutboxDispatch()
        // after each batch if there are remaining messages.
        _hangfire.AddOrUpdateRecurring<OutboxBackgroundJob>(
            jobId: "outbox-dispatcher",
            methodCall: job => job.ExecuteAsync(CancellationToken.None),
            cronExpression: Hangfire.Cron.Minutely(),
            timeZone: TimeZoneInfo.Utc);

        _logger.LogDebug("RecurringJobRegistrar: registered 'outbox-dispatcher' (every minute)");
    }

    // ── Future recurring jobs (add as features grow) ─────────────────────────
    //
    // private void RegisterDailyRetentionSweep() { ... }
    // private void RegisterNightlyArSnapshot() { ... }
    // private void RegisterExpiredSealChecker() { ... }
}
