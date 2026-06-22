using System;
using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Domain.Features.Compliance.Aggregates;

public sealed class Inspection : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public string InspectionCode { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public InspectionStatus Status { get; private set; }
    public IncidentSeverity Severity { get; private set; }
    public Guid? ComplianceRuleId { get; private set; }
    public Guid? LegalHoldId { get; private set; }
    public Guid? IncidentId { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Summary { get; private set; }
    public string? Notes { get; private set; }

    private Inspection()
    {
    }

    private Inspection(Guid id, Guid tenantId, string inspectionCode, string title, IncidentSeverity severity)
        : base(id)
    {
        TenantId = tenantId;
        InspectionCode = inspectionCode;
        Title = title;
        Severity = severity;
        Status = InspectionStatus.Planned;
    }

    public static Inspection Create(
        Guid tenantId,
        string inspectionCode,
        string title,
        IncidentSeverity severity,
        Guid? complianceRuleId = null,
        Guid? legalHoldId = null,
        Guid? incidentId = null,
        DateTime? scheduledAt = null,
        string? summary = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(inspectionCode)) throw new BusinessRuleValidationException("Inspection code is required.");
        if (string.IsNullOrWhiteSpace(title)) throw new BusinessRuleValidationException("Inspection title is required.");

        return new Inspection(Guid.NewGuid(), tenantId, inspectionCode.Trim().ToUpperInvariant(), title.Trim(), severity)
        {
            ComplianceRuleId = complianceRuleId,
            LegalHoldId = legalHoldId,
            IncidentId = incidentId,
            ScheduledAt = scheduledAt,
            Summary = string.IsNullOrWhiteSpace(summary) ? null : summary.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
