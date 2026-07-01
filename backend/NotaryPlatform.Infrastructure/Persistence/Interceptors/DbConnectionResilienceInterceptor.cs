using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace NotaryPlatform.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Observes PostgreSQL connection lifecycle events to surface pool exhaustion,
/// timeouts, and dead-socket reconnections in application logs.
///
/// ── LEARNING: DbConnectionInterceptor lifecycle ────────────────────────────
///
/// There are 4 paired event groups:
///
///   Opening / Opened  — borrowing a connection from the pool
///   Closing / Closed  — returning it to the pool
///   (each pair has sync + async variants)
///   Failed            — the borrow failed (pool exhausted, TCP dead, etc.)
///
/// ── LEARNING: Connection pool vs TCP socket ────────────────────────────────
///
/// ADO.NET and Npgsql maintain a pool of open TCP sockets to PostgreSQL.
///
///   DbConnection.Open()  → borrow a socket from the pool (very fast, ~µs)
///   DbConnection.Close() → return it to the pool (NOT closing the TCP socket)
///
/// A new TCP handshake only happens when:
///   • The pool is empty (no idle connections)
///   • An idle socket was found to be dead (heartbeat failure)
///   • Npgsql's Max Pool Size was reached AND a new request timed out waiting
///
/// So ConnectionOpening fires on EVERY EF query even though no network
/// round-trip occurs most of the time. Log it at Debug level only.
///
/// ── LEARNING: when does ConnectionFailed fire? ────────────────────────────
///
/// ConnectionFailed fires when:
///   1. The pool has no idle connections and creating a new one fails
///      (PostgreSQL is down, firewall blocked, max_connections reached)
///   2. A borrowed idle connection was found dead mid-request
///      (Npgsql will retry once, then throw if both attempts fail)
///
/// In production, a burst of ConnectionFailed events almost always means:
///   a) PostgreSQL is overloaded (check pg_stat_activity connections)
///   b) Network partition between the app server and DB server
///   c) App is leaking connections (connections not returned to the pool)
/// </summary>
public sealed class DbConnectionResilienceInterceptor : DbConnectionInterceptor
{
    private readonly ILogger<DbConnectionResilienceInterceptor> _logger;

    public DbConnectionResilienceInterceptor(ILogger<DbConnectionResilienceInterceptor> logger)
        => _logger = logger;

    // ── Opened ────────────────────────────────────────────────────────────────
    // Log at Debug — fires on every query, noisy but helpful for diagnosing
    // slow pool contention when filtered to a specific correlation ID.

    public override void ConnectionOpened(
        DbConnection connection,
        ConnectionEndEventData eventData)
    {
        _logger.LogDebug(
            "DB connection acquired in {Duration}ms. Database: {Database}",
            (long)eventData.Duration.TotalMilliseconds,
            connection.Database);
    }

    public override Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "DB connection acquired in {Duration}ms. Database: {Database}",
            (long)eventData.Duration.TotalMilliseconds,
            connection.Database);

        return Task.CompletedTask;
    }

    // ── Failed ────────────────────────────────────────────────────────────────
    // Log at Error — actionable: database is unreachable or pool is exhausted.

    public override void ConnectionFailed(
        DbConnection connection,
        ConnectionErrorEventData eventData)
    {
        _logger.LogError(
            eventData.Exception,
            "PostgreSQL connection FAILED. Database: {Database}, Server: {Server}",
            connection.Database,
            connection.DataSource);
    }

    public override Task ConnectionFailedAsync(
        DbConnection connection,
        ConnectionErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _logger.LogError(
            eventData.Exception,
            "PostgreSQL connection FAILED. Database: {Database}, Server: {Server}",
            connection.Database,
            connection.DataSource);

        return Task.CompletedTask;
    }
}
