using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Communication.Events;

public sealed class ThreadClosedDomainEvent : DomainEvent
{
    public Guid CommunicationThreadId { get; }
    public string ThreadCode { get; }

    public ThreadClosedDomainEvent(Guid communicationThreadId, string threadCode)
    {
        CommunicationThreadId = communicationThreadId;
        ThreadCode = threadCode;
    }
}
