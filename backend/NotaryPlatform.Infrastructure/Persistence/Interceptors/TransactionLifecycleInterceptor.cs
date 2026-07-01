using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace NotaryPlatform.Infrastructure.Persistence.Interceptors;

// ── LEARNING: DbTransactionInterceptor — exact method signatures ──────────────
//
// Reading the decompiled EF Core 8 source reveals a non-obvious pattern:
//
//   TransactionStarted / TransactionStartedAsync
//     → first param is DbConnection (where BEGIN was sent)
//     → third param is DbTransaction result  (the new transaction object)
//     → return type is DbTransaction / ValueTask<DbTransaction>  [pass-through]
//
//   TransactionCommitted / TransactionRolledBack / TransactionFailed
//     → first param is DbTransaction (the transaction that just finished)
//     → return type is void / Task  [notification only]
//
// Why the asymmetry?
//   At "Started" time, EF Core is handing you the fresh DbTransaction so you
//   can optionally WRAP or REPLACE it (pass-through pattern).
//   At "Committed/RolledBack" time, the operation is already done — EF passes
//   the transaction for context but you cannot change anything (notification).
//
// ── LEARNING: relationship with TransactionBehavior in the MediatR pipeline ──
//
//   Normal  : TransactionStarted → TransactionCommitted
//   Throws  : TransactionStarted → TransactionRolledBack
//
// A Warning log on RolledBack means a command handler threw and NO data was
// persisted — a useful second signal alongside the application exception log.
//
// ── LEARNING: TransactionFailed vs TransactionRolledBack ─────────────────────
//
// TransactionRolledBack — application code called RollbackAsync intentionally.
// TransactionFailed     — COMMIT or ROLLBACK itself threw at the DB level
//                         (connection dropped mid-commit, disk full, etc.).
//                         Rare; always an infrastructure-level emergency.
public sealed class TransactionLifecycleInterceptor : DbTransactionInterceptor
{
    private readonly ILogger<TransactionLifecycleInterceptor> _logger;

    public TransactionLifecycleInterceptor(ILogger<TransactionLifecycleInterceptor> logger)
        => _logger = logger;

    // ── Started ───────────────────────────────────────────────────────────────
    // Pass-through: first param is DbConnection; third param is the new DbTransaction.

    public override DbTransaction TransactionStarted(
        DbConnection connection,
        TransactionEndEventData eventData,
        DbTransaction result)
    {
        _logger.LogDebug(
            "DB transaction started. IsolationLevel: {IsolationLevel}",
            result.IsolationLevel);

        return result;
    }

    public override ValueTask<DbTransaction> TransactionStartedAsync(
        DbConnection connection,
        TransactionEndEventData eventData,
        DbTransaction result,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "DB transaction started. IsolationLevel: {IsolationLevel}",
            result.IsolationLevel);

        return new ValueTask<DbTransaction>(result);
    }

    // ── Committed ─────────────────────────────────────────────────────────────
    // Notification: first param is the DbTransaction that just committed (not DbConnection).

    public override void TransactionCommitted(
        DbTransaction transaction,
        TransactionEndEventData eventData)
    {
        _logger.LogDebug(
            "DB transaction committed in {Duration}ms",
            (long)eventData.Duration.TotalMilliseconds);
    }

    public override Task TransactionCommittedAsync(
        DbTransaction transaction,
        TransactionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "DB transaction committed in {Duration}ms",
            (long)eventData.Duration.TotalMilliseconds);

        return Task.CompletedTask;
    }

    // ── Rolled back ───────────────────────────────────────────────────────────
    // Notification: first param is DbTransaction (not DbConnection).

    public override void TransactionRolledBack(
        DbTransaction transaction,
        TransactionEndEventData eventData)
    {
        _logger.LogWarning(
            "DB transaction ROLLED BACK after {Duration}ms",
            (long)eventData.Duration.TotalMilliseconds);
    }

    public override Task TransactionRolledBackAsync(
        DbTransaction transaction,
        TransactionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _logger.LogWarning(
            "DB transaction ROLLED BACK after {Duration}ms",
            (long)eventData.Duration.TotalMilliseconds);

        return Task.CompletedTask;
    }

    // ── Failed ────────────────────────────────────────────────────────────────
    // Notification: first param is DbTransaction (not DbConnection).

    public override void TransactionFailed(
        DbTransaction transaction,
        TransactionErrorEventData eventData)
    {
        _logger.LogError(
            eventData.Exception,
            "DB transaction FAILED during {Action}",
            eventData.Action);
    }

    public override Task TransactionFailedAsync(
        DbTransaction transaction,
        TransactionErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _logger.LogError(
            eventData.Exception,
            "DB transaction FAILED during {Action}",
            eventData.Action);

        return Task.CompletedTask;
    }
}
