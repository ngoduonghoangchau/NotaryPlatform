using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.ValueObjects;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class PaymentAllocation : AggregateRoot
{
    public Guid PaymentId { get; private set; }
    public Guid InvoiceId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public string? Notes { get; private set; }

    private PaymentAllocation()
    {
    }

    private PaymentAllocation(Guid id, Guid paymentId, Guid invoiceId, Money amount)
        : base(id)
    {
        PaymentId = paymentId;
        InvoiceId = invoiceId;
        Amount = amount;
    }

    public static PaymentAllocation Create(Guid paymentId, Guid invoiceId, Money amount, string? notes = null)
    {
        if (paymentId == Guid.Empty) throw new BusinessRuleValidationException("Payment id is required.");
        if (invoiceId == Guid.Empty) throw new BusinessRuleValidationException("Invoice id is required.");

        return new PaymentAllocation(Guid.NewGuid(), paymentId, invoiceId, amount)
        {
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
