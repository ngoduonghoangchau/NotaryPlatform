using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Domain.Features.Notarial.Aggregates;

public sealed class ActStatusHistory : AggregateRoot
{
    public Guid NotarialActId { get; private set; }
    public NotarialActStatus PreviousStatus { get; private set; }
    public NotarialActStatus NewStatus { get; private set; }
    public string? Reason { get; private set; }
    public string? ChangedBy { get; private set; }
    public DateTime ChangedAt { get; private set; }

    private ActStatusHistory()
    {
    }

    private ActStatusHistory(Guid id, Guid notarialActId, NotarialActStatus previousStatus, NotarialActStatus newStatus)
        : base(id)
    {
        NotarialActId = notarialActId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        ChangedAt = DateTime.UtcNow;
    }

    public static ActStatusHistory Create(
        Guid notarialActId,
        NotarialActStatus previousStatus,
        NotarialActStatus newStatus,
        string? reason = null,
        string? changedBy = null)
    {
        if (notarialActId == Guid.Empty) throw new BusinessRuleValidationException("Notarial act id is required.");

        return new ActStatusHistory(Guid.NewGuid(), notarialActId, previousStatus, newStatus)
        {
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
            ChangedBy = string.IsNullOrWhiteSpace(changedBy) ? null : changedBy.Trim()
        };
    }
}
