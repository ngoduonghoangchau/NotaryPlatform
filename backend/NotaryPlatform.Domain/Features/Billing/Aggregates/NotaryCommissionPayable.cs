using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.Enums;
using NotaryPlatform.Domain.Features.Billing.ValueObjects;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class NotaryCommissionPayable : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid NotaryId { get; private set; }
    public Guid? InvoiceId { get; private set; }
    public Guid? PaymentId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public PayableStatus Status { get; private set; }
    public Money Amount { get; private set; } = null!;
    public decimal Percentage { get; private set; }
    public DateTime CalculatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? Notes { get; private set; }

    private NotaryCommissionPayable()
    {
    }

    private NotaryCommissionPayable(Guid id, Guid tenantId, Guid notaryId, string code, Money amount, decimal percentage)
        : base(id)
    {
        TenantId = tenantId;
        NotaryId = notaryId;
        Code = code;
        Amount = amount;
        Percentage = percentage;
        Status = PayableStatus.Pending;
        CalculatedAt = DateTime.UtcNow;
    }

    public static NotaryCommissionPayable Create(Guid tenantId, Guid notaryId, string code, Money amount, decimal percentage, Guid? invoiceId = null, Guid? paymentId = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (notaryId == Guid.Empty) throw new BusinessRuleValidationException("Notary id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Payable code is required.");
        if (percentage < 0 || percentage > 100) throw new BusinessRuleValidationException("Percentage must be between 0 and 100.");

        return new NotaryCommissionPayable(Guid.NewGuid(), tenantId, notaryId, code.Trim().ToUpperInvariant(), amount, percentage)
        {
            InvoiceId = invoiceId,
            PaymentId = paymentId,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void MarkPaid()
    {
        Status = PayableStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }
}
