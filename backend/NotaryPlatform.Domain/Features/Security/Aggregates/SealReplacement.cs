using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Domain.Features.Security.Aggregates;

public sealed class SealReplacement : AggregateRoot
{
    public Guid OldSealId { get; private set; }
    public Guid NewSealId { get; private set; }
    public ReplacementReasonType ReasonType { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTime ReplacedAt { get; private set; }
    public Guid? ReplacedByUserId { get; private set; }
    public string? Notes { get; private set; }

    private SealReplacement()
    {
    }

    private SealReplacement(Guid id, Guid oldSealId, Guid newSealId, ReplacementReasonType reasonType, string reason)
        : base(id)
    {
        OldSealId = oldSealId;
        NewSealId = newSealId;
        ReasonType = reasonType;
        Reason = reason;
        ReplacedAt = DateTime.UtcNow;
    }

    public static SealReplacement Create(Guid oldSealId, Guid newSealId, ReplacementReasonType reasonType, string reason, Guid? replacedByUserId = null, string? notes = null)
    {
        if (oldSealId == Guid.Empty) throw new BusinessRuleValidationException("Old seal id is required.");
        if (newSealId == Guid.Empty) throw new BusinessRuleValidationException("New seal id is required.");
        if (string.IsNullOrWhiteSpace(reason)) throw new BusinessRuleValidationException("Replacement reason is required.");

        return new SealReplacement(Guid.NewGuid(), oldSealId, newSealId, reasonType, reason.Trim())
        {
            ReplacedByUserId = replacedByUserId,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
