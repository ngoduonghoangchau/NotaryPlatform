using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Infrastructure.Persistence.Outbox;

namespace NotaryPlatform.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Implements the Transactional Outbox pattern.
/// Before EF Core flushes changes to the database, this interceptor collects
/// all <see cref="Domain.Common.DomainEvents.IDomainEvent"/>s raised by
/// <see cref="AggregateRoot"/> instances currently tracked by the
/// <see cref="DbContext"/>, serialises them as <see cref="OutboxMessage"/>
/// rows, and adds them to the same change-tracker batch so they are persisted
/// in a single atomic transaction.
/// </summary>
public sealed class OutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        ConvertEventsToOutboxMessages(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ConvertEventsToOutboxMessages(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ConvertEventsToOutboxMessages(DbContext? context)
    {
        if (context is null) return;

        var aggregatesWithEvents = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(a => a.DomainEvents.Count > 0)
            .ToList();

        if (aggregatesWithEvents.Count == 0) return;

        var outboxMessages = aggregatesWithEvents
            .SelectMany(a => a.DomainEvents)
            .Select(domainEvent => new OutboxMessage
            {
                Id          = Guid.NewGuid(),
                EventType   = domainEvent.GetType().AssemblyQualifiedName!,
                Payload     = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), JsonOptions),
                OccurredAt  = domainEvent.OccurredOn,
            })
            .ToList();

        // Written in the same change-tracker batch → same DB transaction
        context.Set<OutboxMessage>().AddRange(outboxMessages);

        // Clear events so they are not re-processed if SaveChanges is retried
        foreach (var aggregate in aggregatesWithEvents)
            aggregate.ClearDomainEvents();
    }
}
