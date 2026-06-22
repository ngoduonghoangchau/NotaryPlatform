using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class BillingAuditLog : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid? InvoiceId { get; private set; }
    public Guid? PaymentId { get; private set; }
    public Guid? RefundId { get; private set; }
    public AuditEventType EventType { get; private set; }
    public string? Actor { get; private set; }
    public string? Details { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private BillingAuditLog()
    {
    }

    private BillingAuditLog(Guid id, Guid tenantId, AuditEventType eventType)
        : base(id)
    {
        TenantId = tenantId;
        EventType = eventType;
        OccurredAt = DateTime.UtcNow;
    }

    public static BillingAuditLog Create(Guid tenantId, AuditEventType eventType, Guid? invoiceId = null, Guid? paymentId = null, Guid? refundId = null, string? actor = null, string? details = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");

        return new BillingAuditLog(Guid.NewGuid(), tenantId, eventType)
        {
            InvoiceId = invoiceId,
            PaymentId = paymentId,
            RefundId = refundId,
            Actor = string.IsNullOrWhiteSpace(actor) ? null : actor.Trim(),
            Details = string.IsNullOrWhiteSpace(details) ? null : details.Trim()
        };
    }
}
