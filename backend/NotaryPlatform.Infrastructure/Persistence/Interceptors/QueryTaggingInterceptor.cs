using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Prepends a structured SQL comment to every outgoing database command so
/// that DB-side tools can correlate queries back to a specific tenant/user.
///
/// Result visible in pg_stat_activity:
///   /* app:NotaryPlatform tenant:a1b2… user:c3d4… */
///   SELECT * FROM billing.invoices WHERE ...
///
/// ── LEARNING: *Executing hooks (BEFORE the command runs) ───────────────────
///
/// Unlike the *Executed hooks in SlowQueryInterceptor (which fire AFTER),
/// the *Executing hooks fire BEFORE EF sends the command to PostgreSQL.
/// This gives you the chance to:
///   1. MODIFY  — rewrite command.CommandText, add parameters, change timeout
///   2. REPLACE — return InterceptionResult.SuppressWithResult(myResult) to
///                skip the DB call entirely and return a cached/fake result
///   3. OBSERVE — inspect the SQL for compliance / security checks (then
///                return result unchanged, as this interceptor does)
///
/// ── LEARNING: Singleton vs Scoped services ─────────────────────────────────
///
/// This interceptor is registered as a SINGLETON (one instance per app).
/// ICurrentUser is a SCOPED service (one instance per HTTP request).
/// A singleton must NOT hold a reference to a scoped service directly
/// (called a "captive dependency") — the scoped service would never be
/// released and would retain request-level data across requests.
///
/// The fix: inject IServiceScopeFactory (also a singleton) and resolve
/// ICurrentUser inside a short-lived scope per call. The scope is disposed
/// immediately after reading the values, releasing the scoped instance.
///
/// ── LEARNING: performance trade-off ───────────────────────────────────────
///
/// CreateScope() has a small overhead (~microseconds). Because this runs
/// for EVERY SQL command, consider disabling it in production hot-paths
/// or switching to IHttpContextAccessor (reads HttpContext.User claims
/// directly without creating a DI scope). For a notary compliance platform
/// where auditability matters more than sub-millisecond latency, this cost
/// is usually acceptable.
/// </summary>
public sealed class QueryTaggingInterceptor : DbCommandInterceptor
{
    private readonly IServiceScopeFactory _scopeFactory;

    public QueryTaggingInterceptor(IServiceScopeFactory scopeFactory)
        => _scopeFactory = scopeFactory;

    // ── Reader (SELECT) ───────────────────────────────────────────────────────

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        TagCommand(command);
        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        TagCommand(command);
        return new ValueTask<InterceptionResult<DbDataReader>>(result);
    }

    // ── Non-query (INSERT / UPDATE / DELETE) ──────────────────────────────────

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        TagCommand(command);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        TagCommand(command);
        return new ValueTask<InterceptionResult<int>>(result);
    }

    // ── Scalar (COUNT / EXISTS / MAX …) ───────────────────────────────────────

    public override InterceptionResult<object?> ScalarExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object?> result)
    {
        TagCommand(command);
        return result;
    }

    public override ValueTask<InterceptionResult<object?>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object?> result,
        CancellationToken cancellationToken = default)
    {
        TagCommand(command);
        return new ValueTask<InterceptionResult<object?>>(result);
    }

    // ─────────────────────────────────────────────────────────────────────────

    private void TagCommand(DbCommand command)
    {
        using var scope = _scopeFactory.CreateScope();
        var currentUser = scope.ServiceProvider.GetService<ICurrentUser>();

        var tenantId = currentUser?.TenantId?.ToString() ?? "anon";
        var userId = currentUser?.UserId?.ToString() ?? "anon";

        // PostgreSQL is fine with /* comments */ at the start of any statement.
        // The tag appears verbatim in pg_stat_activity.query and pg_log.
        command.CommandText =
            $"/* app:NotaryPlatform tenant:{tenantId} user:{userId} */\n"
            + command.CommandText;
    }
}
