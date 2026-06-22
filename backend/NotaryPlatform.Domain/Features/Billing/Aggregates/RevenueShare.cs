using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.Enums;
using NotaryPlatform.Domain.Features.Billing.ValueObjects;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class RevenueShare : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid? InvoiceId { get; private set; }
    public Guid? PaymentId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public RevenueShareType RevenueShareType { get; private set; }
    public PayableStatus Status { get; private set; }
    public Money Amount { get; private set; } = null!;
    public decimal Percentage { get; private set; }
    public DateTime CalculatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? Notes { get; private set; }

    private RevenueShare()
    {
    }

    private RevenueShare(Guid id, Guid tenantId, string code, RevenueShareType revenueShareType, Money amount, decimal percentage)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        RevenueShareType = revenueShareType;
        Amount = amount;
        Percentage = percentage;
        Status = PayableStatus.Pending;
        CalculatedAt = DateTime.UtcNow;
    }

    public static RevenueShare Create(Guid tenantId, string code, RevenueShareType revenueShareType, Money amount, decimal percentage, Guid? invoiceId = null, Guid? paymentId = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Revenue share code is required.");
        if (percentage < 0 || percentage > 100) throw new BusinessRuleValidationException("Percentage must be between 0 and 100.");

        return new RevenueShare(Guid.NewGuid(), tenantId, code.Trim().ToUpperInvariant(), revenueShareType, amount, percentage)
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
