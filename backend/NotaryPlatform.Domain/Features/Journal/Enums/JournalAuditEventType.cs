namespace NotaryPlatform.Domain.Features.Journal.Enums;

public enum JournalAuditEventType
{
    Create,
    Update,
    StatusChange,
    Lock,
    Unlock,
    Void,
    Export,
    Transfer,
    Import,
    AttachmentAdded,
    AttachmentRemoved,
    FieldChanged,
    RetentionApplied
}
