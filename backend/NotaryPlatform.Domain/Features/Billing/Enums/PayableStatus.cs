namespace NotaryPlatform.Domain.Features.Billing.Enums;

public enum PayableStatus
{
    Pending,
    Accrued,
    Approved,
    Paid,
    PartiallyPaid,
    Cancelled,
    Reversed
}
