using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Compliance.Events;

public sealed class IncidentOpenedDomainEvent : DomainEvent
{
    public Guid IncidentId { get; }
    public Guid TenantId { get; }
    public string IncidentCode { get; }

    public IncidentOpenedDomainEvent(Guid incidentId, Guid tenantId, string incidentCode)
    {
        IncidentId = incidentId;
        TenantId = tenantId;
        IncidentCode = incidentCode;
    }
}
