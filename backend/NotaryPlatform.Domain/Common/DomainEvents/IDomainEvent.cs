namespace NotaryPlatform.Domain.Common.DomainEvents;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}
