using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.CRM.Events;

public sealed class SlaBreachedDomainEvent : DomainEvent
{
    public Guid SlaAgreementId { get; }
    public Guid CustomerId { get; }
    public string SlaCode { get; }

    public SlaBreachedDomainEvent(Guid slaAgreementId, Guid customerId, string slaCode)
    {
        SlaAgreementId = slaAgreementId;
        CustomerId = customerId;
        SlaCode = slaCode;
    }
}
