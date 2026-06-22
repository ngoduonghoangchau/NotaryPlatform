using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Domain.Features.Compliance.Aggregates;

public sealed class IncidentAction : AggregateRoot
{
    public Guid IncidentId { get; private set; }
    public string ActionCode { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public IncidentStatus Status { get; private set; }
    public IncidentSeverity Severity { get; private set; }
    public DateTime? DueAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public Guid? PerformedByUserId { get; private set; }
    public string? Notes { get; private set; }

    private IncidentAction()
    {
    }

    private IncidentAction(Guid id, Guid incidentId, string actionCode, string title, IncidentSeverity severity)
        : base(id)
    {
        IncidentId = incidentId;
        ActionCode = actionCode;
        Title = title;
        Severity = severity;
        Status = IncidentStatus.Open;
    }

    public static IncidentAction Create(
        Guid incidentId,
        string actionCode,
        string title,
        IncidentSeverity severity,
        DateTime? dueAt = null,
        Guid? assignedToUserId = null,
        string? notes = null)
    {
        if (incidentId == Guid.Empty) throw new BusinessRuleValidationException("Incident id is required.");
        if (string.IsNullOrWhiteSpace(actionCode)) throw new BusinessRuleValidationException("Action code is required.");
        if (string.IsNullOrWhiteSpace(title)) throw new BusinessRuleValidationException("Action title is required.");

        return new IncidentAction(Guid.NewGuid(), incidentId, actionCode.Trim().ToUpperInvariant(), title.Trim(), severity)
        {
            DueAt = dueAt,
            AssignedToUserId = assignedToUserId,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
