using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Identity.Events;

public sealed class CommissionExpiredDomainEvent : DomainEvent
{
    public Guid NotaryCommissionId { get; }
    public Guid NotaryId { get; }
    public DateOnly ExpiredOn { get; }

    public CommissionExpiredDomainEvent(Guid notaryCommissionId, Guid notaryId, DateOnly expiredOn)
    {
        NotaryCommissionId = notaryCommissionId;
        NotaryId = notaryId;
        ExpiredOn = expiredOn;
    }
}
