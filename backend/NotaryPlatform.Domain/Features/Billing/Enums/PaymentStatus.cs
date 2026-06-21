namespace NotaryPlatform.Domain.Features.Billing.Enums;

public enum PaymentStatus
{
    Pending,
    Authorized,
    Captured,
    Settled,
    Failed,
    Reversed,
    Refunded,
    Voided
}
