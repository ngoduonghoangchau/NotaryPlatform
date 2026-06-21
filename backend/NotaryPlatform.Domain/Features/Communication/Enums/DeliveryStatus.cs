namespace NotaryPlatform.Domain.Features.Communication.Enums;

public enum DeliveryStatus
{
    Pending,
    Queued,
    Sent,
    Delivered,
    Failed,
    Bounced,
    Suppressed,
    Cancelled
}
