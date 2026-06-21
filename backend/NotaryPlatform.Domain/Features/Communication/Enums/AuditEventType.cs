namespace NotaryPlatform.Domain.Features.Communication.Enums;

public enum AuditEventType
{
    Create,
    Update,
    Send,
    Deliver,
    Read,
    Fail,
    Cancel,
    OpenThread,
    CloseThread,
    ArchiveThread,
    AddParticipant,
    RemoveParticipant,
    AttachFile,
    RemoveFile,
    CreateNote,
    UpdateNote,
    ScheduleReminder,
    CompleteReminder,
    ScheduleCall,
    CompleteCall
}
