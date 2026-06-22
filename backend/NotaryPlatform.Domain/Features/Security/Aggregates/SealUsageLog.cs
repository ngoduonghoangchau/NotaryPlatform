using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Domain.Features.Security.Aggregates;

public sealed class SealUsageLog : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid SealId { get; private set; }
    public Guid? NotaryId { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? JobId { get; private set; }
    public UsageActionType ActionType { get; private set; }
    public UsageResultType Result { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? Notes { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private SealUsageLog()
    {
    }

    private SealUsageLog(Guid id, Guid tenantId, Guid sealId, UsageActionType actionType, UsageResultType result)
        : base(id)
    {
        TenantId = tenantId;
        SealId = sealId;
        ActionType = actionType;
        Result = result;
        OccurredAt = DateTime.UtcNow;
    }

    public static SealUsageLog Create(
        Guid tenantId,
        Guid sealId,
        UsageActionType actionType,
        UsageResultType result,
        Guid? notaryId = null,
        Guid? userId = null,
        Guid? jobId = null,
        string? externalReference = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (sealId == Guid.Empty) throw new BusinessRuleValidationException("Seal id is required.");

        return new SealUsageLog(Guid.NewGuid(), tenantId, sealId, actionType, result)
        {
            NotaryId = notaryId,
            UserId = userId,
            JobId = jobId,
            ExternalReference = string.IsNullOrWhiteSpace(externalReference) ? null : externalReference.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
