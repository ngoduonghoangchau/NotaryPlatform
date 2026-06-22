using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Operations.Events;

public sealed class ServiceTypeCreatedDomainEvent : DomainEvent
{
    public Guid ServiceTypeId { get; }
    public Guid TenantId { get; }
    public string Code { get; }

    public ServiceTypeCreatedDomainEvent(Guid serviceTypeId, Guid tenantId, string code)
    {
        ServiceTypeId = serviceTypeId;
        TenantId = tenantId;
        Code = code;
    }
}
