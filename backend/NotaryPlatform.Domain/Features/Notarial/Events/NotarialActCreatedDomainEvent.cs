using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Notarial.Events;

public sealed class NotarialActCreatedDomainEvent : DomainEvent
{
    public Guid NotarialActId { get; }
    public Guid TenantId { get; }
    public string ActCode { get; }

    public NotarialActCreatedDomainEvent(Guid notarialActId, Guid tenantId, string actCode)
    {
        NotarialActId = notarialActId;
        TenantId = tenantId;
        ActCode = actCode;
    }
}
