using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Security.Enums;
using NotaryPlatform.Domain.Features.Security.Events;

namespace NotaryPlatform.Domain.Features.Security.Aggregates;

public sealed class SecurityIncident : AggregateRoot
{
    private readonly List<SecurityIncidentAction> _actions = [];

    public Guid TenantId { get; private set; }
    public Guid? SealId { get; private set; }
    public Guid? DigitalCertificateId { get; private set; }
    public Guid? NotaryId { get; private set; }
    public Guid? UserId { get; private set; }
    public IncidentType IncidentType { get; private set; }
    public IncidentSeverity Severity { get; private set; }
    public IncidentStatus Status { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Summary { get; private set; }
    public string? Details { get; private set; }
    public DateTime? DetectedAt { get; private set; }
    public DateTime? ReportedAt { get; private set; }
    public DateTime? ContainedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public Guid? ReportedByUserId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public bool RequiresRegulatoryNotification { get; private set; }
    public DateTime? RegulatoryNotifiedAt { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<SecurityIncidentAction> Actions => _actions.AsReadOnly();

    private SecurityIncident()
    {
    }

    private SecurityIncident(Guid id, Guid tenantId, string title, IncidentType incidentType, IncidentSeverity severity)
        : base(id)
    {
        TenantId = tenantId;
        Title = title;
        IncidentType = incidentType;
        Severity = severity;
        Status = IncidentStatus.Open;
        DetectedAt = DateTime.UtcNow;
    }

    public static SecurityIncident Create(
        Guid tenantId,
        string title,
        IncidentType incidentType,
        IncidentSeverity severity,
        Guid? sealId = null,
        Guid? digitalCertificateId = null,
        Guid? notaryId = null,
        Guid? userId = null,
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

        var incident = new SecurityIncident(Guid.NewGuid(), tenantId, title.Trim(), incidentType, severity)
        {
            SealId = sealId,
            DigitalCertificateId = digitalCertificateId,
            NotaryId = notaryId,
            UserId = userId,
            ReportedByUserId = reportedByUserId,
            AssignedToUserId = assignedToUserId,
            RequiresRegulatoryNotification = requiresRegulatoryNotification,
            Summary = string.IsNullOrWhiteSpace(summary) ? null : summary.Trim(),
            Details = string.IsNullOrWhiteSpace(details) ? null : details.Trim(),
            ExternalReference = string.IsNullOrWhiteSpace(externalReference) ? null : externalReference.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            ReportedAt = reportedByUserId.HasValue ? DateTime.UtcNow : null
        };

        incident.AddDomainEvent(new SecurityIncidentOpenedDomainEvent(incident.Id, tenantId, incident.Title));
        return incident;
    }

    public void UpdateProfile(
        string title,
        IncidentType incidentType,
        IncidentSeverity severity,
        string? summary = null,
        string? details = null,
        string? externalReference = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new BusinessRuleValidationException("Incident title is required.");
        }

        Title = title.Trim();
        IncidentType = incidentType;
        Severity = severity;
        Summary = string.IsNullOrWhiteSpace(summary) ? Summary : summary.Trim();
        Details = string.IsNullOrWhiteSpace(details) ? Details : details.Trim();
        ExternalReference = string.IsNullOrWhiteSpace(externalReference) ? ExternalReference : externalReference.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }
    public void Investigate() => Status = IncidentStatus.Investigating;

    public void Contain()
    {
        Status = IncidentStatus.Contained;
        ContainedAt = DateTime.UtcNow;
    }

    public void Resolve()
    {
        Status = IncidentStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        Status = IncidentStatus.Closed;
        ClosedAt = DateTime.UtcNow;
    }

    public void Escalate() => Status = IncidentStatus.Escalated;

    public void MarkRegulatoryNotified() => RegulatoryNotifiedAt = DateTime.UtcNow;

    public void AssignTo(Guid? assignedToUserId) => AssignedToUserId = assignedToUserId;

    public void AddAction(SecurityIncidentAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (_actions.Exists(x => x.Id == action.Id)) return;
        _actions.Add(action);
    }
}
