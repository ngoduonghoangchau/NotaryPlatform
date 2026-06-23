namespace NotaryPlatform.Application.Common.Interfaces;

/// <summary>
/// Abstracts outbound SMS. Implemented by Infrastructure.Services.Messaging.SmsSender.
/// </summary>
public interface ISmsSender
{
    Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}
