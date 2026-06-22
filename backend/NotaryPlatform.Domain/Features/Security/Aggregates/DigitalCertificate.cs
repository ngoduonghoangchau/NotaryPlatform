using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Security.Enums;
using NotaryPlatform.Domain.Features.Security.Events;
using NotaryPlatform.Domain.Features.Security.ValueObjects;

namespace NotaryPlatform.Domain.Features.Security.Aggregates;

public sealed class DigitalCertificate : AggregateRoot
{
    private readonly List<DigitalCertificate> _chain = [];

    public Guid TenantId { get; private set; }
    public Guid? SealId { get; private set; }
    public Guid? NotaryId { get; private set; }
    public CertificateSerialNumber SerialNumber { get; private set; } = null!;
    public CertificateStatus Status { get; private set; }
    public string? IssuerName { get; private set; }
    public string? SubjectName { get; private set; }
    public string? Thumbprint { get; private set; }
    public DateTime? IssuedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public DateTime? ArchivedAt { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<DigitalCertificate> Chain => _chain.AsReadOnly();

    private DigitalCertificate()
    {
    }

    private DigitalCertificate(Guid id, Guid tenantId, CertificateSerialNumber serialNumber)
        : base(id)
    {
        TenantId = tenantId;
        SerialNumber = serialNumber;
        Status = CertificateStatus.Pending;
    }

    public static DigitalCertificate Create(Guid tenantId, string serialNumber, Guid? sealId = null, Guid? notaryId = null, string? issuerName = null, string? subjectName = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");

        return new DigitalCertificate(Guid.NewGuid(), tenantId, CertificateSerialNumber.Create(serialNumber))
        {
            SealId = sealId,
            NotaryId = notaryId,
            IssuerName = string.IsNullOrWhiteSpace(issuerName) ? null : issuerName.Trim(),
            SubjectName = string.IsNullOrWhiteSpace(subjectName) ? null : subjectName.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            IssuedAt = DateTime.UtcNow
        };
    }

    public void UpdateProfile(string? issuerName = null, string? subjectName = null, string? thumbprint = null, string? notes = null)
    {
        IssuerName = string.IsNullOrWhiteSpace(issuerName) ? IssuerName : issuerName.Trim();
        SubjectName = string.IsNullOrWhiteSpace(subjectName) ? SubjectName : subjectName.Trim();
        Thumbprint = string.IsNullOrWhiteSpace(thumbprint) ? Thumbprint : thumbprint.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void Activate()
    {
        Status = CertificateStatus.Active;
        IssuedAt ??= DateTime.UtcNow;
    }

    public void Suspend(string? reason = null)
    {
        Status = CertificateStatus.Suspended;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Revoke(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BusinessRuleValidationException("Revocation reason is required.");
        }

        Status = CertificateStatus.Revoked;
        RevokedAt = DateTime.UtcNow;
        Notes = reason.Trim();
        AddDomainEvent(new CertificateExpiredDomainEvent(Id, SerialNumber.Value));
    }

    public void Expire(string? reason = null)
    {
        Status = CertificateStatus.Expired;
        ExpiresAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
        AddDomainEvent(new CertificateExpiredDomainEvent(Id, SerialNumber.Value));
    }

    public void Archive() => Status = CertificateStatus.Archived;

    public void AddChainLink(DigitalCertificate certificate)
    {
        ArgumentNullException.ThrowIfNull(certificate);
        if (_chain.Exists(x => x.Id == certificate.Id)) return;
        _chain.Add(certificate);
    }
}
