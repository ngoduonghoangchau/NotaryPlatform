namespace NotaryPlatform.Domain.Features.Security.Enums;

public enum CertificateStatus
{
    Pending,
    Active,
    Expiring,
    Expired,
    Suspended,
    Revoked,
    Replaced,
    Archived
}
