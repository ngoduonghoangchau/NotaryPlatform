using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Notarial.Enums;
using NotaryPlatform.Domain.Features.Notarial.Events;
using NotaryPlatform.Domain.Features.Notarial.ValueObjects;

namespace NotaryPlatform.Domain.Features.Notarial.Aggregates;

public sealed class NotarialCertificate : AggregateRoot
{
    public Guid NotarialActId { get; private set; }
    public CertificateNumber CertificateNumber { get; private set; } = null!;
    public CertificateStatus CertificateStatus { get; private set; }
    public DateTime? GeneratedAt { get; private set; }
    public DateTime? PreviewedAt { get; private set; }
    public DateTime? FinalizedAt { get; private set; }
    public DateTime? LockedAt { get; private set; }
    public DateTime? VoidedAt { get; private set; }
    public string? PdfStorageKey { get; private set; }
    public string? Notes { get; private set; }

    private NotarialCertificate()
    {
    }

    private NotarialCertificate(Guid id, Guid notarialActId, CertificateNumber certificateNumber)
        : base(id)
    {
        NotarialActId = notarialActId;
        CertificateNumber = certificateNumber;
        CertificateStatus = CertificateStatus.Draft;
    }

    public static NotarialCertificate Create(Guid notarialActId, string certificateNumber)
    {
        if (notarialActId == Guid.Empty) throw new BusinessRuleValidationException("Notarial act id is required.");

        return new NotarialCertificate(Guid.NewGuid(), notarialActId, CertificateNumber.Create(certificateNumber));
    }

    public void MarkGenerated(string? pdfStorageKey = null)
    {
        CertificateStatus = CertificateStatus.Generated;
        GeneratedAt = DateTime.UtcNow;
        PdfStorageKey = string.IsNullOrWhiteSpace(pdfStorageKey) ? PdfStorageKey : pdfStorageKey.Trim();
    }

    public void MarkPreviewed() => PreviewedAt = DateTime.UtcNow;

    public void FinalizeCertificate(string? pdfStorageKey = null)
    {
        CertificateStatus = CertificateStatus.Finalized;
        FinalizedAt = DateTime.UtcNow;
        PdfStorageKey = string.IsNullOrWhiteSpace(pdfStorageKey) ? PdfStorageKey : pdfStorageKey.Trim();

        AddDomainEvent(new CertificateFinalizedDomainEvent(NotarialActId, Id, CertificateNumber.Value));
    }

    public void Lock()
    {
        CertificateStatus = CertificateStatus.Locked;
        LockedAt = DateTime.UtcNow;
    }

    public void Void(string? reason = null)
    {
        CertificateStatus = CertificateStatus.Voided;
        VoidedAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }
}
