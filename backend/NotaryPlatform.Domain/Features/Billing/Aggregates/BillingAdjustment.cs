using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.Enums;
using NotaryPlatform.Domain.Features.Billing.ValueObjects;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class BillingAdjustment : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid? InvoiceId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public InvoiceItemType AdjustmentType { get; private set; }
    public AdjustmentStatus Status { get; private set; }
    public Money Amount { get; private set; } = null!;
    public string? Reason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? AppliedAt { get; private set; }
    public string? Notes { get; private set; }

    private BillingAdjustment()
    {
    }

    private BillingAdjustment(Guid id, Guid tenantId, string code, InvoiceItemType adjustmentType, Money amount)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        AdjustmentType = adjustmentType;
        Amount = amount;
        Status = AdjustmentStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public static BillingAdjustment Create(Guid tenantId, string code, InvoiceItemType adjustmentType, Money amount, Guid? invoiceId = null, string? reason = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Adjustment code is required.");

        return new BillingAdjustment(Guid.NewGuid(), tenantId, code.Trim().ToUpperInvariant(), adjustmentType, amount)
        {
            InvoiceId = invoiceId,
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
