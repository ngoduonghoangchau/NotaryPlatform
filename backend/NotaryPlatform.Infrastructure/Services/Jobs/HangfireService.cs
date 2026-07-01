using System.Linq.Expressions;
using Hangfire;

namespace NotaryPlatform.Infrastructure.Services.Jobs;

/// <summary>
/// Thin facade over Hangfire's <see cref="IBackgroundJobClient"/> and
/// <see cref="IRecurringJobManager"/> that provides a single injection point
/// for all job-scheduling operations and simplifies unit testing via mocking.
/// </summary>
public sealed class HangfireService
{
    private readonly IBackgroundJobClient _jobClient;
    private readonly IRecurringJobManager _recurringJobManager;

    public HangfireService(
        IBackgroundJobClient jobClient,
        IRecurringJobManager recurringJobManager)
    {
        _jobClient = jobClient;
        _recurringJobManager = recurringJobManager;
    }

    // ── Fire-and-forget ──────────────────────────────────────────────────────

    /// <summary>Enqueues a one-shot background job for immediate execution.</summary>
    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
        => _jobClient.Enqueue(methodCall);

    /// <summary>Enqueues a one-shot synchronous background job.</summary>
    public string Enqueue<T>(Expression<Action<T>> methodCall)
        => _jobClient.Enqueue(methodCall);

    // ── Delayed ──────────────────────────────────────────────────────────────

    /// <summary>Schedules a job to run after <paramref name="delay"/>.</summary>
    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
        => _jobClient.Schedule(methodCall, delay);

    /// <summary>Schedules a job to run at an absolute UTC instant.</summary>
    public string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset runAt)
        => _jobClient.Schedule(methodCall, runAt);

    // ── Recurring ────────────────────────────────────────────────────────────

    /// <summary>
    /// Adds or replaces a recurring job.
    /// <paramref name="cronExpression"/> follows Hangfire's cron format
    /// (5-field minute-resolution; 6-field second-resolution with Hangfire Pro).
    /// </summary>
    public void AddOrUpdateRecurring<T>(
        string jobId,
        Expression<Func<T, Task>> methodCall,
        string cronExpression,
        TimeZoneInfo? timeZone = null)
    {
        _recurringJobManager.AddOrUpdate(
            jobId,
            methodCall,
            cronExpression,
            new RecurringJobOptions { TimeZone = timeZone ?? TimeZoneInfo.Utc });
    }

    /// <summary>Removes a recurring job if it exists; no-op otherwise.</summary>
    public void RemoveRecurring(string jobId)
        => _recurringJobManager.RemoveIfExists(jobId);

    // ── Continuation ─────────────────────────────────────────────────────────

    /// <summary>
    /// Enqueues <paramref name="continuation"/> to run after the job
    /// identified by <paramref name="parentJobId"/> succeeds.
    /// </summary>
    public string ContinueJobWith<T>(
        string parentJobId,
        Expression<Func<T, Task>> continuation)
        => _jobClient.ContinueJobWith(parentJobId, continuation);

    /// <summary>Deletes a pending job by its Hangfire job ID.</summary>
    public bool Delete(string jobId)
        => _jobClient.Delete(jobId);
}
