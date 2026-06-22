using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Security.Events;

public sealed class SealRevokedDomainEvent : DomainEvent
{
    public Guid SealId { get; }
    public Guid TenantId { get; }
    public string SealCode { get; }

    public SealRevokedDomainEvent(Guid sealId, Guid tenantId, string sealCode)
    {
        SealId = sealId;
        TenantId = tenantId;
        SealCode = sealCode;
    }
}
