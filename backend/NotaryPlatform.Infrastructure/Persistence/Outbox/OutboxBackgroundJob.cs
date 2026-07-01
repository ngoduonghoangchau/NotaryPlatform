using Hangfire;
using Microsoft.Extensions.Logging;

namespace NotaryPlatform.Infrastructure.Persistence.Outbox;

/// <summary>
/// Hangfire recurring job that triggers the outbox dispatcher on a
/// configurable schedule (default: every 30 seconds).
/// <para>
/// Registered by <c>RecurringJobRegistrar</c> at application startup.
/// Hangfire resolves this class from DI per execution, so all its
/// dependencies are properly scoped.
/// </para>
/// </summary>
public sealed class OutboxBackgroundJob
{
    private readonly OutboxDispatcher _dispatcher;
    private readonly ILogger<OutboxBackgroundJob> _logger;

    public OutboxBackgroundJob(
        OutboxDispatcher dispatcher,
        ILogger<OutboxBackgroundJob> logger)
    {
        _dispatcher = dispatcher;
        _logger     = logger;
    }

    /// <summary>
    /// Entry-point invoked by Hangfire.
    /// <see cref="AutomaticRetryAttribute"/> is disabled here because
    /// <see cref="OutboxMessage"/> tracks its own retry state — Hangfire
    /// retrying the whole batch on transient failures would cause duplicates.
    /// </summary>
    [AutomaticRetry(Attempts = 0)]
    [JobDisplayName("Outbox Dispatcher")]
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("OutboxBackgroundJob: starting dispatch cycle");

        try
        {
            await _dispatcher.DispatchPendingAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OutboxBackgroundJob: dispatch cycle failed");
            throw; // Rethrow so Hangfire marks the job as failed in its dashboard
        }
    }
}
