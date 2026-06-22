using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Domain.Features.Communication.Aggregates;

public sealed class CallLog : AggregateRoot
{
    public Guid CommunicationThreadId { get; private set; }
    public Guid? CommunicationMessageId { get; private set; }
    public Guid? AuthorUserId { get; private set; }
    public CallStatus Status { get; private set; }
    public CallOutcome Outcome { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Subject { get; private set; }
    public string? Notes { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public int? DurationSeconds { get; private set; }

    private CallLog()
    {
    }

    private CallLog(Guid id, Guid communicationThreadId, CallStatus status, CallOutcome outcome)
        : base(id)
    {
        CommunicationThreadId = communicationThreadId;
        Status = status;
        Outcome = outcome;
        StartedAt = DateTime.UtcNow;
    }

    public static CallLog Create(
        Guid communicationThreadId,
        CallStatus status,
        CallOutcome outcome,
        Guid? communicationMessageId = null,
        Guid? authorUserId = null,
        string? phoneNumber = null,
        string? subject = null,
        string? notes = null,
        DateTime? startedAt = null,
        DateTime? endedAt = null,
        int? durationSeconds = null)
    {
        if (communicationThreadId == Guid.Empty) throw new BusinessRuleValidationException("Communication thread id is required.");

        return new CallLog(Guid.NewGuid(), communicationThreadId, status, outcome)
        {
            CommunicationMessageId = communicationMessageId,
            AuthorUserId = authorUserId,
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim(),
            Subject = string.IsNullOrWhiteSpace(subject) ? null : subject.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            StartedAt = startedAt.HasValue ? DateTime.SpecifyKind(startedAt.Value, DateTimeKind.Utc) : DateTime.UtcNow,
            EndedAt = endedAt.HasValue ? DateTime.SpecifyKind(endedAt.Value, DateTimeKind.Utc) : null,
            DurationSeconds = durationSeconds
        };
    }
}
