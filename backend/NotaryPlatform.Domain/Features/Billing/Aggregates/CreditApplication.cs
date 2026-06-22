using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.ValueObjects;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class CreditApplication : AggregateRoot
{
    public Guid CreditId { get; private set; }
    public Guid InvoiceId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public DateTime AppliedAt { get; private set; }
    public string? Notes { get; private set; }

    private CreditApplication()
    {
    }

    private CreditApplication(Guid id, Guid creditId, Guid invoiceId, Money amount)
        : base(id)
    {
        CreditId = creditId;
        InvoiceId = invoiceId;
        Amount = amount;
        AppliedAt = DateTime.UtcNow;
    }

    public static CreditApplication Create(Guid creditId, Guid invoiceId, Money amount, string? notes = null)
    {
        if (creditId == Guid.Empty) throw new BusinessRuleValidationException("Credit id is required.");
        if (invoiceId == Guid.Empty) throw new BusinessRuleValidationException("Invoice id is required.");

        return new CreditApplication(Guid.NewGuid(), creditId, invoiceId, amount)
        {
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
