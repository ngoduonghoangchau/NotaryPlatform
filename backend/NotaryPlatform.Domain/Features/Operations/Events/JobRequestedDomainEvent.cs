using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Operations.Events;

public sealed class JobRequestedDomainEvent : DomainEvent
{
    public Guid JobRequestId { get; }
    public Guid TenantId { get; }
    public string JobCode { get; }

    public JobRequestedDomainEvent(Guid jobRequestId, Guid tenantId, string jobCode)
    {
        JobRequestId = jobRequestId;
        TenantId = tenantId;
        JobCode = jobCode;
    }
}
