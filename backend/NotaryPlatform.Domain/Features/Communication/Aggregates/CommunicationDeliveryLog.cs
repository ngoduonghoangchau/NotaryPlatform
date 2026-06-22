using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Domain.Features.Communication.Aggregates;

public sealed class CommunicationDeliveryLog : AggregateRoot
{
    public Guid CommunicationMessageId { get; private set; }
    public DeliveryStatus DeliveryStatus { get; private set; }
    public string? Channel { get; private set; }
    public string? Recipient { get; private set; }
    public string? ProviderReference { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime AttemptedAt { get; private set; }

    private CommunicationDeliveryLog()
    {
    }

    private CommunicationDeliveryLog(Guid id, Guid communicationMessageId, DeliveryStatus deliveryStatus)
        : base(id)
    {
        CommunicationMessageId = communicationMessageId;
        DeliveryStatus = deliveryStatus;
        AttemptedAt = DateTime.UtcNow;
    }

    public static CommunicationDeliveryLog Create(
        Guid communicationMessageId,
        DeliveryStatus deliveryStatus,
        string? channel = null,
        string? recipient = null,
        string? providerReference = null,
        string? errorMessage = null)
    {
        if (communicationMessageId == Guid.Empty) throw new BusinessRuleValidationException("Communication message id is required.");

        return new CommunicationDeliveryLog(Guid.NewGuid(), communicationMessageId, deliveryStatus)
        {
            Channel = string.IsNullOrWhiteSpace(channel) ? null : channel.Trim(),
            Recipient = string.IsNullOrWhiteSpace(recipient) ? null : recipient.Trim(),
            ProviderReference = string.IsNullOrWhiteSpace(providerReference) ? null : providerReference.Trim(),
            ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) ? null : errorMessage.Trim()
        };
    }
}
