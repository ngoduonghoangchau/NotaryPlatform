using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Billing.Events;

public sealed class InvoiceIssuedDomainEvent : DomainEvent
{
    public Guid InvoiceId { get; }
    public Guid TenantId { get; }
    public string InvoiceNumber { get; }

    public InvoiceIssuedDomainEvent(Guid invoiceId, Guid tenantId, string invoiceNumber)
    {
        InvoiceId = invoiceId;
        TenantId = tenantId;
        InvoiceNumber = invoiceNumber;
    }
}
