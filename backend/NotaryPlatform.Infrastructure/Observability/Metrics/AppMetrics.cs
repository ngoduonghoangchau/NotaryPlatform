using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace NotaryPlatform.Infrastructure.Observability.Metrics;

/// <summary>
/// Central registry of OpenTelemetry instruments used throughout the application.
///
/// LEARNING — Meter vs ActivitySource:
///   <see cref="Meter"/>        → metrics (counters, histograms, gauges).
///   <see cref="ActivitySource"/> → distributed traces (spans).
///   Both share the same <c>NotaryPlatform</c> name so OTel backends can
///   correlate metrics with their corresponding traces.
///
/// LEARNING — Static instrument instances:
///   Instruments are cheap to create but expensive to look up by name.
///   Declaring them as static fields creates them once at startup and keeps
///   them available throughout the process lifetime — the recommended pattern.
///
/// LEARNING — Naming convention (OpenTelemetry semantic conventions):
///   {namespace}.{entity}.{verb_or_noun}  (snake_case, dot-separated namespaces)
///   Examples: "notary.jobs.created", "notary.dispatch.duration"
/// </summary>
public static class AppMetrics
{
    // ── Identity ──────────────────────────────────────────────────────────────────

    /// <summary>Name used to register the <see cref="Meter"/> with OpenTelemetry.</summary>
    public const string MeterName = "NotaryPlatform";

    /// <summary>Name used to register the <see cref="ActivitySource"/> with OpenTelemetry.</summary>
    public const string ActivitySourceName = "NotaryPlatform";

    /// <summary>
    /// Application-wide <see cref="ActivitySource"/> for creating distributed-trace spans.
    /// Use: <c>using var activity = AppMetrics.ActivitySource.StartActivity("DoWork");</c>
    /// </summary>
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName, "1.0.0");

    // ── Meter ─────────────────────────────────────────────────────────────────────

    private static readonly Meter _meter = new(MeterName, "1.0.0");

    // ── Operations / Jobs ─────────────────────────────────────────────────────────

    /// <summary>
    /// Total jobs created. Dimensions: <c>service_type</c>, <c>tenant_id</c>.
    /// </summary>
    public static readonly Counter<long> JobsCreated =
        _meter.CreateCounter<long>(
            "notary.jobs.created",
            description: "Number of job requests created.");

    /// <summary>
    /// Total jobs completed (any terminal status). Dimensions: <c>status</c>, <c>tenant_id</c>.
    /// </summary>
    public static readonly Counter<long> JobsCompleted =
        _meter.CreateCounter<long>(
            "notary.jobs.completed",
            description: "Number of jobs that reached a terminal status.");

    /// <summary>
    /// Time from job creation to Notary assignment (dispatch latency).
    /// </summary>
    public static readonly Histogram<double> DispatchDuration =
        _meter.CreateHistogram<double>(
            "notary.dispatch.duration",
            unit: "ms",
            description: "Elapsed time from job creation to Notary assignment.");

    // ── Notarial Acts ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Total notarial acts executed. Dimensions: <c>act_type</c>, <c>state_code</c>.
    /// </summary>
    public static readonly Counter<long> NotarialActsExecuted =
        _meter.CreateCounter<long>(
            "notary.acts.executed",
            description: "Number of notarial acts executed.");

    /// <summary>
    /// Total notarial acts voided. Dimensions: <c>act_type</c>, <c>tenant_id</c>.
    /// </summary>
    public static readonly Counter<long> NotarialActsVoided =
        _meter.CreateCounter<long>(
            "notary.acts.voided",
            description: "Number of notarial acts voided.");

    // ── Compliance ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Number of compliance check failures. Dimensions: <c>rule_code</c>, <c>tenant_id</c>.
    /// A spike here indicates legal risk and should trigger an alert.
    /// </summary>
    public static readonly Counter<long> ComplianceCheckFailures =
        _meter.CreateCounter<long>(
            "notary.compliance.failures",
            description: "Number of compliance check rule violations.");

    // ── Caching ───────────────────────────────────────────────────────────────────

    /// <summary>Cache hits. Dimensions: <c>feature</c>.</summary>
    public static readonly Counter<long> CacheHits =
        _meter.CreateCounter<long>(
            "notary.cache.hits",
            description: "Number of cache reads that returned a cached value.");

    /// <summary>Cache misses. Dimensions: <c>feature</c>.</summary>
    public static readonly Counter<long> CacheMisses =
        _meter.CreateCounter<long>(
            "notary.cache.misses",
            description: "Number of cache reads that required a database query.");

    // ── Auth ──────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Failed authentication attempts. Dimensions: <c>reason</c>.
    /// Useful for detecting brute-force attacks.
    /// </summary>
    public static readonly Counter<long> AuthFailures =
        _meter.CreateCounter<long>(
            "notary.auth.failures",
            description: "Number of failed authentication or authorisation attempts.");
}
