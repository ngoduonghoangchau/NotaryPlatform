using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;

namespace NotaryPlatform.Infrastructure.Persistence.Outbox;

/// <summary>
/// Reads the next batch of unprocessed <see cref="OutboxMessage"/> rows from
/// the database and delegates each one to <see cref="OutboxMessageProcessor"/>
/// for deserialisation and MediatR dispatch.
/// <para>
/// Called by <see cref="OutboxBackgroundJob"/> on a Hangfire recurring schedule.
/// Each message is committed (or failure-stamped) individually so progress is
/// not lost if the batch is interrupted mid-way.
/// </para>
/// </summary>
public sealed class OutboxDispatcher
{
    public const int MaxRetries = 5;
    private const int BatchSize = 50;

    private readonly NotaryPlatformDbContext _db;
    private readonly OutboxMessageProcessor _processor;
    private readonly ILogger<OutboxDispatcher> _logger;

    public OutboxDispatcher(
        NotaryPlatformDbContext db,
        OutboxMessageProcessor processor,
        ILogger<OutboxDispatcher> logger)
    {
        _db        = db;
        _processor = processor;
        _logger    = logger;
    }

    public async Task DispatchPendingAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _db.Set<OutboxMessage>()
            .Where(m => m.ProcessedAt == null && m.RetryCount < MaxRetries)
            .OrderBy(m => m.OccurredAt)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0) return;

        _logger.LogDebug("Outbox: dispatching {Count} pending message(s)", messages.Count);

        foreach (var message in messages)
        {
            try
            {
                await _processor.ProcessAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                // ProcessAsync already stamps RetryCount / Error; log and continue
                _logger.LogError(ex, "Outbox: unhandled error processing message {Id}", message.Id);
            }

            // Commit after each message so a crash only replays the remaining ones
            await _db.SaveChangesAsync(cancellationToken);
        }

        _logger.LogDebug("Outbox: batch complete ({Count} message(s) processed)", messages.Count);
    }
}
