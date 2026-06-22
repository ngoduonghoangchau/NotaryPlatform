using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.CRM.Events;

public sealed class CustomerCreatedDomainEvent : DomainEvent
{
    public Guid CustomerId { get; }
    public Guid TenantId { get; }
    public string CustomerCode { get; }

    public CustomerCreatedDomainEvent(Guid customerId, Guid tenantId, string customerCode)
    {
        CustomerId = customerId;
        TenantId = tenantId;
        CustomerCode = customerCode;
    }
}
