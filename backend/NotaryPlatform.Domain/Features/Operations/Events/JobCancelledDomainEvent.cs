using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Operations.Events;

public sealed class JobCancelledDomainEvent : DomainEvent
{
    public Guid JobId { get; }
    public string Reason { get; }

    public JobCancelledDomainEvent(Guid jobId, string reason)
    {
        JobId = jobId;
        Reason = reason;
    }
}
