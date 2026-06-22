using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Billing.Events;

public sealed class RefundProcessedDomainEvent : DomainEvent
{
    public Guid RefundId { get; }
    public Guid PaymentId { get; }
    public string? Reason { get; }

    public RefundProcessedDomainEvent(Guid refundId, Guid paymentId, string? reason)
    {
        RefundId = refundId;
        PaymentId = paymentId;
        Reason = reason;
    }
}
