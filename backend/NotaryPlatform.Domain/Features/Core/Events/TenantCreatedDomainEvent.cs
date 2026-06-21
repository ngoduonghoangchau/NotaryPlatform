using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Core.Events;

public sealed class TenantCreatedDomainEvent : DomainEvent
{
    public Guid TenantId { get; }
    public string TenantCode { get; }
    public string TenantName { get; }

    public TenantCreatedDomainEvent(Guid tenantId, string tenantCode, string tenantName)
    {
        TenantId = tenantId;
        TenantCode = tenantCode;
        TenantName = tenantName;
    }
}
