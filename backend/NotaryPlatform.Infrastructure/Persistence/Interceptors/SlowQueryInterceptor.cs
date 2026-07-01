using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace NotaryPlatform.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Detects and logs any SQL command that exceeds a configurable execution-time
/// threshold — essential for catching N+1 queries and missing indexes in production.
///
/// ── LEARNING: DbCommandInterceptor lifecycle ───────────────────────────────
///
/// DbCommandInterceptor exposes two phases for every SQL command:
///
///   BEFORE (Executing hooks) — runs before the command hits the DB:
///     ReaderExecuting[Async]   → SELECT that returns rows
///     NonQueryExecuting[Async] → INSERT / UPDATE / DELETE
///     ScalarExecuting[Async]   → single-value SELECT (COUNT, EXISTS, MAX…)
///     Return InterceptionResult.SuppressWithResult(...) to short-circuit the DB call.
///     Modify command.CommandText here if you want to rewrite SQL on the fly.
///
///   AFTER (Executed hooks) — runs after the DB returns:
///     ReaderExecuted[Async]    → receives the open DbDataReader
///     NonQueryExecuted[Async]  → receives the rows-affected int
///     ScalarExecuted[Async]    → receives the scalar object?
///     eventData.Duration holds the wall-clock time EF measured.
///     Return a different result to substitute what the caller sees.
///
/// ── LEARNING: interceptor execution order ──────────────────────────────────
///
/// EF Core calls interceptors in the order they are registered via
/// .AddInterceptors(). The *Executing chain runs first-to-last; the
/// *Executed chain runs last-to-first (like ASP.NET middleware unwinding).
///
/// ── LEARNING: why warn and not throw? ─────────────────────────────────────
///
/// A slow query is a performance problem, not a correctness one. Throwing
/// would break the user's request for no functional reason. Logging at
/// Warning means the on-call team can alert on it without user impact.
/// </summary>
public sealed class SlowQueryInterceptor : DbCommandInterceptor
{
    private static readonly TimeSpan DefaultThreshold = TimeSpan.FromMilliseconds(500);

    private readonly ILogger<SlowQueryInterceptor> _logger;
    private readonly TimeSpan _threshold;

    public SlowQueryInterceptor(ILogger<SlowQueryInterceptor> logger, TimeSpan? threshold = null)
    {
        _logger = logger;
        _threshold = threshold ?? DefaultThreshold;
    }

    // ── Reader (SELECT) ───────────────────────────────────────────────────────

    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        LogIfSlow(command, eventData.Duration);
        return result;
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        LogIfSlow(command, eventData.Duration);
        return new ValueTask<DbDataReader>(result);
    }

    // ── Non-query (INSERT / UPDATE / DELETE) ──────────────────────────────────

    public override int NonQueryExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result)
    {
        LogIfSlow(command, eventData.Duration);
        return result;
    }

    public override ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        LogIfSlow(command, eventData.Duration);
        return new ValueTask<int>(result);
    }

    // ── Scalar (COUNT / EXISTS / MAX …) ───────────────────────────────────────

    public override object? ScalarExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result)
    {
        LogIfSlow(command, eventData.Duration);
        return result;
    }

    public override ValueTask<object?> ScalarExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result,
        CancellationToken cancellationToken = default)
    {
        LogIfSlow(command, eventData.Duration);
        return new ValueTask<object?>(result);
    }

    // ─────────────────────────────────────────────────────────────────────────

    private void LogIfSlow(DbCommand command, TimeSpan duration)
    {
        if (duration < _threshold) return;

        // CommandText is safe to log because EF Core always uses parameterised
        // queries — parameter values are NOT inlined into the SQL text.
        _logger.LogWarning(
            "Slow SQL detected ({Duration}ms exceeds {Threshold}ms threshold):\n{Sql}",
            (long)duration.TotalMilliseconds,
            (long)_threshold.TotalMilliseconds,
            command.CommandText);
    }
}
