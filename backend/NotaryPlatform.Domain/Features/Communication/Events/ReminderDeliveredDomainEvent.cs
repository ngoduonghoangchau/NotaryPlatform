using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Communication.Events;

public sealed class ReminderDeliveredDomainEvent : DomainEvent
{
    public Guid CommunicationReminderId { get; }
    public string ReminderCode { get; }

    public ReminderDeliveredDomainEvent(Guid communicationReminderId, string reminderCode)
    {
        CommunicationReminderId = communicationReminderId;
        ReminderCode = reminderCode;
    }
}
