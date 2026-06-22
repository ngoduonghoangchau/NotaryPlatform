using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Compliance.Events;

public sealed class LegalHoldAppliedDomainEvent : DomainEvent
{
    public Guid LegalHoldId { get; }
    public Guid TenantId { get; }
    public string HoldCode { get; }

    public LegalHoldAppliedDomainEvent(Guid legalHoldId, Guid tenantId, string holdCode)
    {
        LegalHoldId = legalHoldId;
        TenantId = tenantId;
        HoldCode = holdCode;
    }
}
