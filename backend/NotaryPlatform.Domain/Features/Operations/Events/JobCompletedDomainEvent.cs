using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Operations.Events;

public sealed class JobCompletedDomainEvent : DomainEvent
{
    public Guid JobId { get; }
    public string JobCode { get; }

    public JobCompletedDomainEvent(Guid jobId, string jobCode)
    {
        JobId = jobId;
        JobCode = jobCode;
    }
}
