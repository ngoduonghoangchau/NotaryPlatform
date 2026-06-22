using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Domain.Features.Notarial.Aggregates;

public sealed class ActExecutionRecord : AggregateRoot
{
    public Guid NotarialActId { get; private set; }
    public ExecutionStatus ExecutionStatus { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Venue { get; private set; }
    public string? Notes { get; private set; }

    private ActExecutionRecord()
    {
    }

    private ActExecutionRecord(Guid id, Guid notarialActId)
        : base(id)
    {
        NotarialActId = notarialActId;
        ExecutionStatus = ExecutionStatus.NotStarted;
    }

    public static ActExecutionRecord Create(Guid notarialActId, string? venue = null, string? notes = null)
    {
        if (notarialActId == Guid.Empty) throw new BusinessRuleValidationException("Notarial act id is required.");

        return new ActExecutionRecord(Guid.NewGuid(), notarialActId)
        {
            Venue = string.IsNullOrWhiteSpace(venue) ? null : venue.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void Start(string? venue = null)
    {
        ExecutionStatus = ExecutionStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        Venue = string.IsNullOrWhiteSpace(venue) ? Venue : venue.Trim();
    }

    public void Complete(string? notes = null)
    {
        ExecutionStatus = ExecutionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void Abandon(string? reason = null)
    {
        ExecutionStatus = ExecutionStatus.Abandoned;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void MarkRequiresReview(string? notes = null)
    {
        ExecutionStatus = ExecutionStatus.RequiresReview;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }
}
