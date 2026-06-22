using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Security.Events;

public sealed class SecurityIncidentOpenedDomainEvent : DomainEvent
{
    public Guid SecurityIncidentId { get; }
    public Guid TenantId { get; }
    public string Title { get; }

    public SecurityIncidentOpenedDomainEvent(Guid securityIncidentId, Guid tenantId, string title)
    {
        SecurityIncidentId = securityIncidentId;
        TenantId = tenantId;
        Title = title;
    }
}
