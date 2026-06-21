namespace NotaryPlatform.Domain.Features.Notarial.Enums;

public enum NotarialActStatus
{
    Draft,
    PendingVerification,
    InExecution,
    AwaitingCertificate,
    AwaitingJournal,
    Completed,
    Locked,
    Voided,
    Cancelled,
    Archived
}
