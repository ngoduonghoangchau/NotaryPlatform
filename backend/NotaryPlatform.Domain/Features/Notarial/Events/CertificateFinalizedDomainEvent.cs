using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Notarial.Events;

public sealed class CertificateFinalizedDomainEvent : DomainEvent
{
    public Guid NotarialActId { get; }
    public Guid NotarialCertificateId { get; }
    public string CertificateNumber { get; }

    public CertificateFinalizedDomainEvent(Guid notarialActId, Guid notarialCertificateId, string certificateNumber)
    {
        NotarialActId = notarialActId;
        NotarialCertificateId = notarialCertificateId;
        CertificateNumber = certificateNumber;
    }
}
