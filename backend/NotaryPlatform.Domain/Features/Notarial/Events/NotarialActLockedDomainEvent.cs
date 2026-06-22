using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Notarial.Events;

public sealed class NotarialActLockedDomainEvent : DomainEvent
{
    public Guid NotarialActId { get; }
    public string? Reason { get; }

    public NotarialActLockedDomainEvent(Guid notarialActId, string? reason)
    {
        NotarialActId = notarialActId;
        Reason = reason;
    }
}
