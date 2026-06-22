using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.Enums;
using NotaryPlatform.Domain.Features.Billing.ValueObjects;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class InvoiceItem : AggregateRoot
{
    public Guid InvoiceId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public InvoiceItemType ItemType { get; private set; }
    public decimal Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public Money LineTotal { get; private set; } = null!;
    public string? Notes { get; private set; }

    private InvoiceItem()
    {
    }

    private InvoiceItem(Guid id, Guid invoiceId, string description, InvoiceItemType itemType, decimal quantity, Money unitPrice)
        : base(id)
    {
        InvoiceId = invoiceId;
        Description = description;
        ItemType = itemType;
        Quantity = quantity;
        UnitPrice = unitPrice;
        LineTotal = Money.Create(quantity * unitPrice.Amount, unitPrice.Currency);
    }

    public static InvoiceItem Create(Guid invoiceId, string description, InvoiceItemType itemType, decimal quantity, Money unitPrice, string? notes = null)
    {
        if (invoiceId == Guid.Empty) throw new BusinessRuleValidationException("Invoice id is required.");
        if (string.IsNullOrWhiteSpace(description)) throw new BusinessRuleValidationException("Invoice item description is required.");
        if (quantity <= 0) throw new BusinessRuleValidationException("Quantity must be greater than zero.");

        return new InvoiceItem(Guid.NewGuid(), invoiceId, description.Trim(), itemType, quantity, unitPrice)
        {
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void Update(decimal quantity, Money unitPrice, string? notes = null)
    {
        if (quantity <= 0) throw new BusinessRuleValidationException("Quantity must be greater than zero.");

        Quantity = quantity;
        UnitPrice = unitPrice;
        LineTotal = Money.Create(quantity * unitPrice.Amount, unitPrice.Currency);
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }
}
