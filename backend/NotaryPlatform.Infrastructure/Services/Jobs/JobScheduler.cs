using NotaryPlatform.Application.Common.Interfaces;
using NotaryPlatform.Infrastructure.Persistence.Outbox;

namespace NotaryPlatform.Infrastructure.Services.Jobs;

/// <summary>
/// Feature-level scheduling API used by Application use cases to enqueue
/// infrastructure jobs without depending on Hangfire.
/// Implements IJobScheduler (Application) — callers never reference this class directly.
///
/// LEARNING — Why a JobScheduler wrapper over HangfireService?
///   Application use cases should express business intent ("schedule a reminder for
///   this appointment") without knowing HOW that intent is executed (Hangfire, Quartz,
///   Azure Service Bus delayed messages, etc.). This class translates business intent
///   into Hangfire calls. If we ever switch job frameworks, only this class changes.
///   Application handlers call IJobScheduler — they never import Hangfire types.
///
/// LEARNING — Method naming:
///   Method names express BUSINESS INTENT (ScheduleReminder, TriggerOutboxDispatch),
///   not technical operations (Enqueue, Schedule). Callers read as business prose.
/// </summary>
public sealed class JobScheduler : IJobScheduler
{
    private readonly HangfireService _hangfire;

    public JobScheduler(HangfireService hangfire) => _hangfire = hangfire;

    // ── Outbox ───────────────────────────────────────────────────────────────────

    public string TriggerOutboxDispatch() =>
        _hangfire.Enqueue<OutboxBackgroundJob>(job => job.ExecuteAsync(CancellationToken.None));

    // ── Retention ────────────────────────────────────────────────────────────────

    public string ScheduleRetentionPurge(
        Guid tenantId,
        string entityType,
        Guid entityId,
        TimeSpan? delay = null)
    {
        // RetentionPurgeJob will be added under Persistence/Jobs when the
        // Compliance.RetentionPolicy feature is implemented.
        throw new NotImplementedException(
            "RetentionPurgeJob not yet implemented. Add the job class and wire it here.");
    }

    // ── Notifications ─────────────────────────────────────────────────────────────

    public string ScheduleReminder(Guid reminderId, DateTimeOffset sendAt)
    {
        // ReminderDispatchJob will be added when the Communication feature is implemented.
        throw new NotImplementedException(
            "ReminderDispatchJob not yet implemented. Add the job class and wire it here.");
    }
}
