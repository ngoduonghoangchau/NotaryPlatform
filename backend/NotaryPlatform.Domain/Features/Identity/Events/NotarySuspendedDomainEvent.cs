using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Identity.Events;

public sealed class NotarySuspendedDomainEvent : DomainEvent
{
    public Guid NotaryId { get; }
    public string Reason { get; }

    public NotarySuspendedDomainEvent(Guid notaryId, string reason)
    {
        NotaryId = notaryId;
        Reason = reason;
    }
}
