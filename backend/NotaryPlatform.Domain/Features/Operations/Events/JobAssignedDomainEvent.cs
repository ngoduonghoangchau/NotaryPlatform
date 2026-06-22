using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Operations.Events;

public sealed class JobAssignedDomainEvent : DomainEvent
{
    public Guid JobId { get; }
    public Guid JobAssignmentId { get; }
    public Guid NotaryId { get; }

    public JobAssignedDomainEvent(Guid jobId, Guid jobAssignmentId, Guid notaryId)
    {
        JobId = jobId;
        JobAssignmentId = jobAssignmentId;
        NotaryId = notaryId;
    }
}
