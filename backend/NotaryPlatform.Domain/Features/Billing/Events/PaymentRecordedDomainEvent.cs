using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Billing.Events;

public sealed class PaymentRecordedDomainEvent : DomainEvent
{
    public Guid PaymentId { get; }
    public Guid InvoiceId { get; }
    public string PaymentCode { get; }

    public PaymentRecordedDomainEvent(Guid paymentId, Guid invoiceId, string paymentCode)
    {
        PaymentId = paymentId;
        InvoiceId = invoiceId;
        PaymentCode = paymentCode;
    }
}
