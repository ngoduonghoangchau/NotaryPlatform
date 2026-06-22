using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Domain.Features.Security.Aggregates;

public sealed class SealRevocation : AggregateRoot
{
    public Guid SealId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public ReplacementReasonType ReasonType { get; private set; }
    public DateTime RevokedAt { get; private set; }
    public Guid? RevokedByUserId { get; private set; }
    public string? Notes { get; private set; }

    private SealRevocation()
    {
    }

    private SealRevocation(Guid id, Guid sealId, string reason, ReplacementReasonType reasonType)
        : base(id)
    {
        SealId = sealId;
        Reason = reason;
        ReasonType = reasonType;
        RevokedAt = DateTime.UtcNow;
    }

    public static SealRevocation Create(Guid sealId, string reason, ReplacementReasonType reasonType, Guid? revokedByUserId = null, string? notes = null)
    {
        if (sealId == Guid.Empty) throw new BusinessRuleValidationException("Seal id is required.");
        if (string.IsNullOrWhiteSpace(reason)) throw new BusinessRuleValidationException("Revocation reason is required.");

        return new SealRevocation(Guid.NewGuid(), sealId, reason.Trim(), reasonType)
        {
            RevokedByUserId = revokedByUserId,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
