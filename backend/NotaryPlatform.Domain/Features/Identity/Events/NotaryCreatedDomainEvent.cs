using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Identity.Events;

public sealed class NotaryCreatedDomainEvent : DomainEvent
{
    public Guid NotaryId { get; }
    public Guid TenantId { get; }
    public Guid UserId { get; }

    public NotaryCreatedDomainEvent(Guid notaryId, Guid tenantId, Guid userId)
    {
        NotaryId = notaryId;
        TenantId = tenantId;
        UserId = userId;
    }
}
