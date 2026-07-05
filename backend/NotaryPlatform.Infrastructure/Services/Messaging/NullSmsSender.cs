using Microsoft.Extensions.Logging;
using NotaryPlatform.Application.Abstractions.Messaging;

namespace NotaryPlatform.Infrastructure.Services.Messaging;

/// <summary>
/// No-op <see cref="ISmsSender"/> used when SMS is turned off (<c>Sms:Disabled = true</c>) —
/// e.g. the zero-budget profile that has no paid SMS provider. It logs that the message was
/// suppressed and returns success, so notification flows keep working (email + push still fire)
/// without anyone having to null-check the sender.
/// </summary>
public sealed class NullSmsSender : ISmsSender
{
    private readonly ILogger<NullSmsSender> _logger;

    public NullSmsSender(ILogger<NullSmsSender> logger) => _logger = logger;

    public Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        // Do not log the message body — it may contain OTPs / PII.
        _logger.LogInformation("SMS suppressed (Sms:Disabled=true); would have sent to {Phone}.", phoneNumber);
        return Task.CompletedTask;
    }
}
