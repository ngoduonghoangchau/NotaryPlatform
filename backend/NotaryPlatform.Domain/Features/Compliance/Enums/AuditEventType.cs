namespace NotaryPlatform.Domain.Features.Compliance.Enums;

public enum AuditEventType
{
    Create,
    Update,
    Delete,
    StatusChange,
    RuleEvaluated,
    CheckStarted,
    CheckCompleted,
    CheckFailed,
    Blocked,
    Unblocked,
    HoldApplied,
    HoldReleased,
    PolicyVersioned,
    RetentionApplied,
    RetentionScheduled,
    ExportRequested,
    ExportGenerated,
    InspectionStarted,
    InspectionCompleted,
    IncidentOpened,
    IncidentUpdated,
    IncidentClosed
}
