namespace NotaryPlatform.Domain.Features.Compliance.Enums;

public enum IncidentType
{
    PolicyViolation,
    StateRuleViolation,
    UnauthorizedAccess,
    MissingJournalEntry,
    ExpiredSealUsage,
    ExpiredCertificateUsage,
    LateFiling,
    DataRetentionViolation,
    PrivacyViolation,
    Other
}
