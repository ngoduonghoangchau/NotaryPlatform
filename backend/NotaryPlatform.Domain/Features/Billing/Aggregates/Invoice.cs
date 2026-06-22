using System.Linq;
using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.Enums;
using NotaryPlatform.Domain.Features.Billing.Events;
using NotaryPlatform.Domain.Features.Billing.ValueObjects;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class Invoice : AggregateRoot
{
    private readonly List<InvoiceItem> _items = new();
    private readonly List<PaymentAllocation> _paymentAllocations = new();
    private readonly List<CreditApplication> _creditApplications = new();

    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? ContractId { get; private set; }
    public Guid? SlaAgreementId { get; private set; }
    public InvoiceNumber InvoiceNumber { get; private set; } = null!;
    public InvoiceStatus Status { get; private set; }
    public DateOnly IssueDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public DateOnly? PaidOn { get; private set; }
    public DateOnly? VoidedOn { get; private set; }
    public string? Currency { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal AmountPaid { get; private set; }
    public decimal AmountDue { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<PaymentAllocation> PaymentAllocations => _paymentAllocations.AsReadOnly();
    public IReadOnlyCollection<CreditApplication> CreditApplications => _creditApplications.AsReadOnly();

    private Invoice()
    {
    }

    private Invoice(Guid id, Guid tenantId, Guid customerId, InvoiceNumber invoiceNumber, DateOnly issueDate, DateOnly dueDate)
        : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        InvoiceNumber = invoiceNumber;
        IssueDate = issueDate;
        DueDate = dueDate;
        Status = InvoiceStatus.Draft;
    }

    public static Invoice Create(Guid tenantId, Guid customerId, string invoiceNumber, DateOnly issueDate, DateOnly dueDate, Guid? contractId = null, Guid? slaAgreementId = null, string? currency = "USD", string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (customerId == Guid.Empty) throw new BusinessRuleValidationException("Customer id is required.");
        if (dueDate < issueDate) throw new BusinessRuleValidationException("Due date must be greater than or equal to issue date.");

        return new Invoice(Guid.NewGuid(), tenantId, customerId, InvoiceNumber.Create(invoiceNumber), issueDate, dueDate)
        {
            ContractId = contractId,
            SlaAgreementId = slaAgreementId,
            Currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void AddItem(InvoiceItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (_items.Exists(x => x.Id == item.Id)) return;
        _items.Add(item);
        RecalculateTotals();
    }

    public void RemoveItem(Guid invoiceItemId)
    {
        _items.RemoveAll(x => x.Id == invoiceItemId);
        RecalculateTotals();
    }

    public void UpdateSchedule(DateOnly issueDate, DateOnly dueDate)
    {
        if (dueDate < issueDate)
        {
            throw new BusinessRuleValidationException("Due date must be greater than or equal to issue date.");
        }

        IssueDate = issueDate;
        DueDate = dueDate;
    }

    public void Issue()
    {
        Status = InvoiceStatus.Issued;
        AddDomainEvent(new InvoiceIssuedDomainEvent(Id, TenantId, InvoiceNumber.Value));
    }
    public void MarkPartiallyPaid() => Status = InvoiceStatus.PartiallyPaid;

    public void MarkPaid(DateOnly paidOn)
    {
        Status = InvoiceStatus.Paid;
        PaidOn = paidOn;
        AmountPaid = TotalAmount;
        AmountDue = 0;
    }

    public void MarkOverdue() => Status = InvoiceStatus.Overdue;

    public void Void(string? reason = null)
    {
        Status = InvoiceStatus.Voided;
        VoidedOn = DateOnly.FromDateTime(DateTime.UtcNow);
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void WriteOff(string? reason = null)
    {
        Status = InvoiceStatus.WrittenOff;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Cancel(string? reason = null)
    {
        Status = InvoiceStatus.Cancelled;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void ApplyPayment(decimal amount)
    {
        if (amount < 0) throw new BusinessRuleValidationException("Payment amount cannot be negative.");

        AmountPaid = decimal.Round(AmountPaid + amount, 2);
        AmountDue = decimal.Round(Math.Max(0, TotalAmount - AmountPaid), 2);

        if (AmountDue == 0 && TotalAmount > 0)
        {
            Status = InvoiceStatus.Paid;
        }
        else if (AmountPaid > 0)
        {
            Status = InvoiceStatus.PartiallyPaid;
        }
    }

    public void ApplyCredit(decimal amount)
    {
        if (amount < 0) throw new BusinessRuleValidationException("Credit amount cannot be negative.");

        AmountPaid = decimal.Round(AmountPaid + amount, 2);
        AmountDue = decimal.Round(Math.Max(0, TotalAmount - AmountPaid), 2);
    }

    public void AddPaymentAllocation(PaymentAllocation paymentAllocation)
    {
        ArgumentNullException.ThrowIfNull(paymentAllocation);
        if (_paymentAllocations.Exists(x => x.Id == paymentAllocation.Id)) return;
        _paymentAllocations.Add(paymentAllocation);
    }

    public void AddCreditApplication(CreditApplication creditApplication)
    {
        ArgumentNullException.ThrowIfNull(creditApplication);
        if (_creditApplications.Exists(x => x.Id == creditApplication.Id)) return;
        _creditApplications.Add(creditApplication);
    }

    private void RecalculateTotals()
    {
        Subtotal = decimal.Round(_items.Sum(x => x.LineTotal.Amount), 2);
        TotalAmount = decimal.Round(Subtotal + TaxAmount - DiscountAmount, 2);
        AmountDue = decimal.Round(Math.Max(0, TotalAmount - AmountPaid), 2);
    }
}
