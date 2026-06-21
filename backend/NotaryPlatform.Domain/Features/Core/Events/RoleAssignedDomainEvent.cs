using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Core.Events;

public sealed class RoleAssignedDomainEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid RoleId { get; }
    public Guid TenantId { get; }

    public RoleAssignedDomainEvent(Guid userId, Guid roleId, Guid tenantId)
    {
        UserId = userId;
        RoleId = roleId;
        TenantId = tenantId;
    }
}
