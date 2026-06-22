using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.Enums;
using NotaryPlatform.Domain.Features.Billing.Events;
using NotaryPlatform.Domain.Features.Billing.ValueObjects;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class Refund : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid PaymentId { get; private set; }
    public Guid? InvoiceId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public RefundStatus Status { get; private set; }
    public Money Amount { get; private set; } = null!;
    public string? Reason { get; private set; }
    public string? ProviderReference { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? FailedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? Notes { get; private set; }

    private Refund()
    {
    }

    private Refund(Guid id, Guid tenantId, Guid paymentId, string code, Money amount)
        : base(id)
    {
        TenantId = tenantId;
        PaymentId = paymentId;
        Code = code;
        Amount = amount;
        Status = RefundStatus.Pending;
        RequestedAt = DateTime.UtcNow;
    }

    public static Refund Create(Guid tenantId, Guid paymentId, string code, Money amount, Guid? invoiceId = null, string? reason = null, string? providerReference = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (paymentId == Guid.Empty) throw new BusinessRuleValidationException("Payment id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Refund code is required.");

        return new Refund(Guid.NewGuid(), tenantId, paymentId, code.Trim().ToUpperInvariant(), amount)
        {
            InvoiceId = invoiceId,
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
            ProviderReference = string.IsNullOrWhiteSpace(providerReference) ? null : providerReference.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void Process(string? providerReference = null, string? notes = null)
    {
        Status = RefundStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
        ProviderReference = string.IsNullOrWhiteSpace(providerReference) ? ProviderReference : providerReference.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
        AddDomainEvent(new RefundProcessedDomainEvent(Id, PaymentId, Reason));
    }

    public void Fail(string? notes = null)
    {
        Status = RefundStatus.Failed;
        FailedAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void Cancel(string? notes = null)
    {
        Status = RefundStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }
}
