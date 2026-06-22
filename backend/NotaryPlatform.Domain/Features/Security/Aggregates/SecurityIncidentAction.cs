using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Domain.Features.Security.Aggregates;

public sealed class SecurityIncidentAction : AggregateRoot
{
    public Guid SecurityIncidentId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public IncidentStatus Status { get; private set; }
    public DateTime? DueAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public Guid? PerformedByUserId { get; private set; }
    public string? Notes { get; private set; }

    private SecurityIncidentAction()
    {
    }

    private SecurityIncidentAction(Guid id, Guid securityIncidentId, string code, string title)
        : base(id)
    {
        SecurityIncidentId = securityIncidentId;
        Code = code;
        Title = title;
        Status = IncidentStatus.Open;
    }

    public static SecurityIncidentAction Create(Guid securityIncidentId, string code, string title, string? description = null, DateTime? dueAt = null, Guid? assignedToUserId = null, string? notes = null)
    {
        if (securityIncidentId == Guid.Empty) throw new BusinessRuleValidationException("Security incident id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Action code is required.");
        if (string.IsNullOrWhiteSpace(title)) throw new BusinessRuleValidationException("Action title is required.");

        return new SecurityIncidentAction(Guid.NewGuid(), securityIncidentId, code.Trim().ToUpperInvariant(), title.Trim())
        {
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            DueAt = dueAt,
            AssignedToUserId = assignedToUserId,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void Assign(Guid? assignedToUserId) => AssignedToUserId = assignedToUserId;

    public void MarkInvestigating() => Status = IncidentStatus.Investigating;

    public void Complete(Guid? performedByUserId = null, string? notes = null)
    {
        Status = IncidentStatus.Resolved;
        CompletedAt = DateTime.UtcNow;
        PerformedByUserId = performedByUserId;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }
}
