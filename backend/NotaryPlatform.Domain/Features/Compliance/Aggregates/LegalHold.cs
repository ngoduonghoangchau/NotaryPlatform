using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Compliance.Enums;
using NotaryPlatform.Domain.Features.Compliance.Events;
using NotaryPlatform.Domain.Features.Compliance.ValueObjects;

namespace NotaryPlatform.Domain.Features.Compliance.Aggregates;

public sealed class LegalHold : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public HoldCode HoldCode { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public LegalHoldStatus Status { get; private set; }
    public Guid? ComplianceRuleId { get; private set; }
    public Guid? IncidentId { get; private set; }
    public Guid? InspectionId { get; private set; }
    public DateTime AppliedAt { get; private set; }
    public DateTime? ReleasedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? Reason { get; private set; }
    public string? Notes { get; private set; }

    private LegalHold()
    {
    }

    private LegalHold(Guid id, Guid tenantId, HoldCode holdCode, string name)
        : base(id)
    {
        TenantId = tenantId;
        HoldCode = holdCode;
        Name = name;
        Status = LegalHoldStatus.Active;
        AppliedAt = DateTime.UtcNow;
    }

    public static LegalHold Create(
        Guid tenantId,
        string holdCode,
        string name,
        Guid? complianceRuleId = null,
        Guid? incidentId = null,
        Guid? inspectionId = null,
        DateTime? expiresAt = null,
        string? reason = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Legal hold name is required.");

        var hold = new LegalHold(Guid.NewGuid(), tenantId, HoldCode.Create(holdCode), name.Trim())
        {
            ComplianceRuleId = complianceRuleId,
            IncidentId = incidentId,
            InspectionId = inspectionId,
            ExpiresAt = expiresAt,
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        hold.AddDomainEvent(new LegalHoldAppliedDomainEvent(hold.Id, tenantId, hold.HoldCode.Value));
        return hold;
    }
}
