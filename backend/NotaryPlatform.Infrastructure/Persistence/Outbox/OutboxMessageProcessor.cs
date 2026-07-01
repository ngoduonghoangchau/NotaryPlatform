using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Infrastructure.Persistence.Outbox;

/// <summary>
/// Deserialises a single <see cref="OutboxMessage"/> and publishes it as a
/// MediatR notification.
/// <para>
/// Because <see cref="IDomainEvent"/> does not extend
/// <see cref="INotification"/>, the event is wrapped in a
/// <see cref="DomainEventNotification{T}"/> before publishing. Application
/// handlers register against <c>INotificationHandler&lt;DomainEventNotification&lt;T&gt;&gt;</c>.
/// </para>
/// </summary>
public sealed class OutboxMessageProcessor
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IPublisher _publisher;
    private readonly ILogger<OutboxMessageProcessor> _logger;

    public OutboxMessageProcessor(IPublisher publisher, ILogger<OutboxMessageProcessor> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task ProcessAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var eventType = ResolveType(message.EventType);
            if (eventType is null)
            {
                _logger.LogWarning(
                    "Outbox: unknown event type '{EventType}' for message {Id} — skipping",
                    message.EventType, message.Id);

                MarkFailed(message, $"Unknown event type: {message.EventType}");
                return;
            }


            if (JsonSerializer.Deserialize(message.Payload, eventType, JsonOptions) is not IDomainEvent domainEvent)
            {
                MarkFailed(message, $"Deserialised payload is null for type {eventType.Name}");
                return;
            }

            // Wrap in a MediatR INotification so Application handlers can subscribe
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(eventType);
            var notification = (INotification)Activator.CreateInstance(notificationType, domainEvent)!;

            await _publisher.Publish(notification, cancellationToken);

            message.ProcessedAt = DateTime.UtcNow;
            message.Error = null;
        }
        catch (Exception ex)
        {
            message.RetryCount++;
            message.Error = ex.ToString();

            _logger.LogError(ex,
                "Outbox: failed to process message {Id} (attempt {Attempt}/{Max})",
                message.Id, message.RetryCount, OutboxDispatcher.MaxRetries);
        }
    }

    private static void MarkFailed(OutboxMessage message, string reason)
    {
        message.RetryCount = OutboxDispatcher.MaxRetries; // no more retries
        message.Error = reason;
        message.ProcessedAt = DateTime.UtcNow;
    }

    private static Type? ResolveType(string assemblyQualifiedName)
        => Type.GetType(assemblyQualifiedName, throwOnError: false);
}

/// <summary>
/// Generic MediatR notification wrapper for domain events.
/// Application layers handle <c>INotificationHandler&lt;DomainEventNotification&lt;T&gt;&gt;</c>
/// to react to specific domain events published through the outbox.
/// </summary>
public sealed record DomainEventNotification<T>(T Event) : INotification
    where T : IDomainEvent;
