using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using NotaryPlatform.Application.Common.Interfaces;
using FcmMessage = FirebaseAdmin.Messaging.Message;
using FcmNotification = FirebaseAdmin.Messaging.Notification;

namespace NotaryPlatform.Infrastructure.Services.Messaging;

/// <summary>
/// Push notification sender using Firebase Cloud Messaging (FCM).
/// Implements IPushNotificationSender (Application) — callers never reference this class directly.
///
/// LEARNING — Why inject FirebaseMessaging instead of using FirebaseMessaging.DefaultInstance?
///   FirebaseMessaging.DefaultInstance is a static singleton — a hidden dependency.
///   Injecting it explicitly:
///   • Makes the dependency visible in the constructor (easier to reason about).
///   • Allows tests to provide a different FirebaseApp instance.
///   • Follows DIP: the class declares what it needs; DI wires it.
///   Registration: services.AddSingleton(_ => FirebaseMessaging.DefaultInstance)
///
/// LEARNING — FCM targeting modes:
///   Device token : Send to one specific device installation (registration token).
///   Topic        : Pub/sub — clients subscribe; server publishes to the topic.
///   Condition    : Boolean expression across topics.
///
/// LEARNING — SendToUserAsync strategy (topic-per-user):
///   FCM has no native "send to user ID" concept. We use a dedicated topic per user:
///   "user_{userId:N}". When a device registers, subscribe it to this topic via:
///     FirebaseMessaging.SubscribeToTopicAsync([deviceToken], "user_{userId:N}")
///   Then SendToUserAsync delegates to SendToTopicAsync — all devices receive it.
/// </summary>
public sealed class PushNotificationSender : IPushNotificationSender
{
    private readonly FirebaseMessaging _messaging;
    private readonly ILogger<PushNotificationSender> _logger;

    public PushNotificationSender(
        FirebaseMessaging messaging,
        ILogger<PushNotificationSender> logger)
    {
        _messaging = messaging;
        _logger = logger;
    }

    public async Task SendToDeviceAsync(
        string deviceToken,
        PushNotification notification,
        CancellationToken cancellationToken = default)
    {
        var message = BuildMessage(notification);
        message.Token = deviceToken;

        var messageId = await _messaging.SendAsync(message, cancellationToken);
        _logger.LogDebug("FCM sent to device. MessageId: {MessageId}", messageId);
    }

    public async Task SendToTopicAsync(
        string topic,
        PushNotification notification,
        CancellationToken cancellationToken = default)
    {
        var message = BuildMessage(notification);
        message.Topic = topic;

        var messageId = await _messaging.SendAsync(message, cancellationToken);
        _logger.LogDebug("FCM sent to topic '{Topic}'. MessageId: {MessageId}", topic, messageId);
    }

    public Task SendToUserAsync(
        Guid userId,
        PushNotification notification,
        CancellationToken cancellationToken = default) =>
        SendToTopicAsync($"user_{userId:N}", notification, cancellationToken);

    private static FcmMessage BuildMessage(PushNotification n)
    {
        var message = new FcmMessage
        {
            Notification = new FcmNotification { Title = n.Title, Body = n.Body },
            Android = new AndroidConfig
            {
                Priority = Priority.High,
                Notification = new AndroidNotification { Sound = n.Sound },
            },
            Apns = new ApnsConfig
            {
                Aps = new Aps { Sound = n.Sound, Badge = n.BadgeCount },
            },
        };

        if (n.Data is { } data)
            message.Data = new Dictionary<string, string>(data);

        return message;
    }
}
