namespace NotaryPlatform.Application.Abstractions.Messaging;

/// <summary>
/// Abstracts outbound SMS. Implemented by Infrastructure.Services.Messaging.SmsSender.
/// </summary>
public interface ISmsSender
{
    Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}
