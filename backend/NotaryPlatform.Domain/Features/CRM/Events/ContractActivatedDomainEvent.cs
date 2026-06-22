using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.CRM.Events;

public sealed class ContractActivatedDomainEvent : DomainEvent
{
    public Guid ContractId { get; }
    public Guid CustomerId { get; }
    public string ContractNumber { get; }

    public ContractActivatedDomainEvent(Guid contractId, Guid customerId, string contractNumber)
    {
        ContractId = contractId;
        CustomerId = customerId;
        ContractNumber = contractNumber;
    }
}
