using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Compliance.Enums;
using NotaryPlatform.Domain.Features.Compliance.Events;
using NotaryPlatform.Domain.Features.Compliance.ValueObjects;

namespace NotaryPlatform.Domain.Features.Compliance.Aggregates;

public sealed class Incident : AggregateRoot
{
    private readonly List<IncidentAction> _actions = new();

    public Guid TenantId { get; private set; }
    public IncidentCode IncidentCode { get; private set; } = null!;
    public string Title { get; private set; } = string.Empty;
    public IncidentType IncidentType { get; private set; }
    public IncidentSeverity Severity { get; private set; }
    public IncidentStatus Status { get; private set; }
    public Guid? ComplianceRuleId { get; private set; }
    public Guid? LegalHoldId { get; private set; }
    public Guid? InspectionId { get; private set; }
    public Guid? RegulatoryExportId { get; private set; }
    public Guid? ReportedByUserId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public bool RequiresRegulatoryNotification { get; private set; }
    public DateTime DetectedAt { get; private set; }
    public DateTime? ReportedAt { get; private set; }
    public DateTime? ContainedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public string? Summary { get; private set; }
    public string? Details { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<IncidentAction> Actions => _actions.AsReadOnly();

    private Incident()
    {
    }

    private Incident(Guid id, Guid tenantId, IncidentCode incidentCode, string title, IncidentType incidentType, IncidentSeverity severity)
        : base(id)
    {
        TenantId = tenantId;
        IncidentCode = incidentCode;
        Title = title;
        IncidentType = incidentType;
        Severity = severity;
        Status = IncidentStatus.Open;
        DetectedAt = DateTime.UtcNow;
    }

    public static Incident Create(
        Guid tenantId,
        string incidentCode,
        string title,
        IncidentType incidentType,
        IncidentSeverity severity,
        Guid? complianceRuleId = null,
        Guid? legalHoldId = null,
        Guid? inspectionId = null,
        Guid? regulatoryExportId = null,
        Guid? reportedByUserId = null,
        Guid? assignedToUserId = null,
        bool requiresRegulatoryNotification = false,
        string? summary = null,
        string? details = null,
        string? externalReference = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(title)) throw new BusinessRuleValidationException("Incident title is required.");

        var incident = new Incident(Guid.NewGuid(), tenantId, IncidentCode.Create(incidentCode), title.Trim(), incidentType, severity)
        {
            ComplianceRuleId = complianceRuleId,
            LegalHoldId = legalHoldId,
            InspectionId = inspectionId,
            RegulatoryExportId = regulatoryExportId,
            ReportedByUserId = reportedByUserId,
            AssignedToUserId = assignedToUserId,
            RequiresRegulatoryNotification = requiresRegulatoryNotification,
            Summary = string.IsNullOrWhiteSpace(summary) ? null : summary.Trim(),
            Details = string.IsNullOrWhiteSpace(details) ? null : details.Trim(),
            ExternalReference = string.IsNullOrWhiteSpace(externalReference) ? null : externalReference.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            ReportedAt = reportedByUserId.HasValue ? DateTime.UtcNow : null
        };

        incident.AddDomainEvent(new IncidentOpenedDomainEvent(incident.Id, tenantId, incident.IncidentCode.Value));
        return incident;
    }

    public void AddAction(IncidentAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (_actions.Exists(x => x.Id == action.Id)) return;
        _actions.Add(action);
    }
}
