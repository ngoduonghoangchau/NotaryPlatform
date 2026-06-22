using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Communication.Enums;
using NotaryPlatform.Domain.Features.Communication.Events;

namespace NotaryPlatform.Domain.Features.Communication.Aggregates;

public sealed class CommunicationReminder : AggregateRoot
{
    public Guid CommunicationThreadId { get; private set; }
    public Guid? CommunicationMessageId { get; private set; }
    public string ReminderCode { get; private set; } = string.Empty;
    public ReminderStatus Status { get; private set; }
    public DateTime DueAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? Channel { get; private set; }
    public string? Notes { get; private set; }

    private CommunicationReminder()
    {
    }

    private CommunicationReminder(Guid id, Guid communicationThreadId, string reminderCode, DateTime dueAt)
        : base(id)
    {
        CommunicationThreadId = communicationThreadId;
        ReminderCode = reminderCode;
        DueAt = DateTime.SpecifyKind(dueAt, DateTimeKind.Utc);
        Status = ReminderStatus.Pending;
    }

    public static CommunicationReminder Create(
        Guid communicationThreadId,
        string reminderCode,
        DateTime dueAt,
        Guid? communicationMessageId = null,
        string? channel = null,
        string? notes = null)
    {
        if (communicationThreadId == Guid.Empty) throw new BusinessRuleValidationException("Communication thread id is required.");
        if (string.IsNullOrWhiteSpace(reminderCode)) throw new BusinessRuleValidationException("Reminder code is required.");

        return new CommunicationReminder(Guid.NewGuid(), communicationThreadId, reminderCode.Trim().ToUpperInvariant(), dueAt)
        {
            CommunicationMessageId = communicationMessageId,
            Channel = string.IsNullOrWhiteSpace(channel) ? null : channel.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
