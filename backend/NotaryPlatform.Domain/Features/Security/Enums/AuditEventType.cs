namespace NotaryPlatform.Domain.Features.Security.Enums;

public enum AuditEventType
{
    Create,
    Update,
    StatusChange,
    Activate,
    Suspend,
    Revoke,
    Replace,
    Lock,
    Unlock,
    UsageAllowed,
    UsageDenied,
    IncidentOpened,
    IncidentUpdated,
    IncidentClosed,
    PolicyChanged,
    DeviceTrusted,
    DeviceRevoked,
    MfaAdded,
    MfaRemoved
}
