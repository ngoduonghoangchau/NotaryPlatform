namespace NotaryPlatform.Domain.Features.Notarial.Enums;

public enum ActEventType
{
    Create,
    Update,
    StatusChange,
    VerificationAdded,
    ExecutionStarted,
    ExecutionCompleted,
    CertificateGenerated,
    CertificateFinalized,
    JournalLinked,
    Locked,
    Voided,
    Unlocked,
    DocumentAttached,
    DocumentRemoved
}
