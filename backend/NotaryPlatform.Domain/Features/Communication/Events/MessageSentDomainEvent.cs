using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Communication.Events;

public sealed class MessageSentDomainEvent : DomainEvent
{
    public Guid CommunicationMessageId { get; }
    public Guid CommunicationThreadId { get; }
    public string MessageCode { get; }

    public MessageSentDomainEvent(Guid communicationMessageId, Guid communicationThreadId, string messageCode)
    {
        CommunicationMessageId = communicationMessageId;
        CommunicationThreadId = communicationThreadId;
        MessageCode = messageCode;
    }
}
