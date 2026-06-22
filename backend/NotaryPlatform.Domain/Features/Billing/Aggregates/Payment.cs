using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.Enums;
using NotaryPlatform.Domain.Features.Billing.Events;
using NotaryPlatform.Domain.Features.Billing.ValueObjects;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class Payment : AggregateRoot
{
    private readonly List<PaymentAllocation> _allocations = new();
    private readonly List<CreditApplication> _creditApplications = new();

    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? InvoiceId { get; private set; }
    public Guid? PaymentMethodId { get; private set; }
    public PaymentCode PaymentCode { get; private set; } = null!;
    public PaymentStatus PaymentStatus { get; private set; }
    public Money Amount { get; private set; } = null!;
    public Money? FeeAmount { get; private set; }
    public Money? NetAmount { get; private set; }
    public DateTime PaidAt { get; private set; }
    public string? ProviderTransactionId { get; private set; }
    public string? ProviderReference { get; private set; }
    public string? ReceiptNumber { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<PaymentAllocation> Allocations => _allocations.AsReadOnly();
    public IReadOnlyCollection<CreditApplication> CreditApplications => _creditApplications.AsReadOnly();

    private Payment()
    {
    }

    private Payment(Guid id, Guid tenantId, Guid customerId, PaymentCode paymentCode, Money amount)
        : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        PaymentCode = paymentCode;
        Amount = amount;
        PaymentStatus = PaymentStatus.Pending;
        PaidAt = DateTime.UtcNow;
    }

    public static Payment Create(
        Guid tenantId,
        Guid customerId,
        string paymentCode,
        Money amount,
        Guid? invoiceId = null,
        Guid? paymentMethodId = null,
        string? providerTransactionId = null,
        string? providerReference = null,
        string? receiptNumber = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (customerId == Guid.Empty) throw new BusinessRuleValidationException("Customer id is required.");

        var payment = new Payment(Guid.NewGuid(), tenantId, customerId, PaymentCode.Create(paymentCode), amount)
        {
            InvoiceId = invoiceId,
            PaymentMethodId = paymentMethodId,
            ProviderTransactionId = string.IsNullOrWhiteSpace(providerTransactionId) ? null : providerTransactionId.Trim(),
            ProviderReference = string.IsNullOrWhiteSpace(providerReference) ? null : providerReference.Trim(),
            ReceiptNumber = string.IsNullOrWhiteSpace(receiptNumber) ? null : receiptNumber.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            NetAmount = amount
        };

        payment.AddDomainEvent(new PaymentRecordedDomainEvent(payment.Id, invoiceId ?? Guid.Empty, payment.PaymentCode.Value));
        return payment;
    }

    public void Fail(string? reason = null)
    {
        PaymentStatus = PaymentStatus.Failed;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Void(string? reason = null)
    {
        PaymentStatus = PaymentStatus.Voided;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Refund(string? reason = null)
    {
        PaymentStatus = PaymentStatus.Refunded;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void SetFees(Money? feeAmount)
    {
        FeeAmount = feeAmount;
        NetAmount = feeAmount is null ? Amount : Amount.Subtract(feeAmount);
    }

    public void AddAllocation(PaymentAllocation allocation)
    {
        ArgumentNullException.ThrowIfNull(allocation);
        if (_allocations.Exists(x => x.Id == allocation.Id)) return;
        _allocations.Add(allocation);
    }

    public void AddCreditApplication(CreditApplication creditApplication)
    {
        ArgumentNullException.ThrowIfNull(creditApplication);
        if (_creditApplications.Exists(x => x.Id == creditApplication.Id)) return;
        _creditApplications.Add(creditApplication);
    }
}
