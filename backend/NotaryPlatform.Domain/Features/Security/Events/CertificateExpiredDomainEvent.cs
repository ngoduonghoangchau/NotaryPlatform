using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Security.Events;

public sealed class CertificateExpiredDomainEvent : DomainEvent
{
    public Guid DigitalCertificateId { get; }
    public string SerialNumber { get; }

    public CertificateExpiredDomainEvent(Guid digitalCertificateId, string serialNumber)
    {
        DigitalCertificateId = digitalCertificateId;
        SerialNumber = serialNumber;
    }
}
