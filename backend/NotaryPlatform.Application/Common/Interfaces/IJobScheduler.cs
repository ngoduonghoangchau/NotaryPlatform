namespace NotaryPlatform.Application.Common.Interfaces;

/// <summary>
/// Schedules background jobs from Application use cases without depending
/// on any specific job queue technology (Hangfire, Quartz, etc.).
/// Implement this interface in the Infrastructure layer.
/// </summary>
public interface IJobScheduler
{
    /// <summary>
    /// Immediately enqueues a one-shot outbox dispatch cycle.
    /// Useful after a command completes to reduce event propagation latency
    /// below the next scheduled recurring cycle.
    /// Returns the job ID assigned by the underlying queue.
    /// </summary>
    string TriggerOutboxDispatch();

    /// <summary>
    /// Schedules a data retention purge or archive operation for a specific entity.
    /// Runs after <paramref name="delay"/> (default: 1 hour).
    /// Returns the scheduled job ID.
    /// </summary>
    string ScheduleRetentionPurge(
        Guid tenantId,
        string entityType,
        Guid entityId,
        TimeSpan? delay = null);

    /// <summary>
    /// Schedules a reminder notification to be dispatched at <paramref name="sendAt"/>.
    /// Returns the scheduled job ID.
    /// </summary>
    string ScheduleReminder(Guid reminderId, DateTimeOffset sendAt);
}
