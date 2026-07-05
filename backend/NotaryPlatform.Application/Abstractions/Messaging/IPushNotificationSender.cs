namespace NotaryPlatform.Application.Abstractions.Messaging;

/// <summary>
/// Abstracts Firebase Cloud Messaging push notifications.
/// Implemented by Infrastructure.Services.Messaging.PushNotificationSender.
/// </summary>
public interface IPushNotificationSender
{
    Task SendToDeviceAsync(string deviceToken, PushNotification notification, CancellationToken cancellationToken = default);
    Task SendToTopicAsync(string topic, PushNotification notification, CancellationToken cancellationToken = default);

    /// <summary>Sends to all devices registered by a given user ID.</summary>
    Task SendToUserAsync(Guid userId, PushNotification notification, CancellationToken cancellationToken = default);
}

public sealed record PushNotification
{
    public required string Title { get; init; }
    public required string Body { get; init; }
    public IDictionary<string, string>? Data { get; init; }

    /// <summary>Badge count for iOS.</summary>
    public int? BadgeCount { get; init; }

    /// <summary>Sound file name. "default" uses the OS default.</summary>
    public string Sound { get; init; } = "default";
}
