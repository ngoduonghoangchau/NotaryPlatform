namespace NotaryPlatform.Domain.Features.Billing.Enums;

public enum InvoiceStatus
{
    Draft,
    Issued,
    PartiallyPaid,
    Paid,
    Overdue,
    Voided,
    Cancelled,
    WrittenOff
}
