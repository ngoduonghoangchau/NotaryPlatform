using Microsoft.Extensions.Logging;
using NotaryPlatform.Application.Abstractions.Messaging;

namespace NotaryPlatform.Infrastructure.Services.Messaging;

/// <summary>
/// No-op <see cref="IEmailSender"/> used when email is turned off (<c>Email:Disabled = true</c>) —
/// e.g. the zero-budget / local-dev profile that has no SMTP server. It logs that the message was
/// suppressed (recipient + subject only — <b>never</b> the body, which may carry reset links / PII,
/// matching <see cref="NullSmsSender"/>'s discipline) and returns success so notification flows keep
/// working without callers having to null-check the sender.
/// </summary>
public sealed class NullEmailSender : IEmailSender
{
    private readonly ILogger<NullEmailSender> _logger;

    public NullEmailSender(ILogger<NullEmailSender> logger) => _logger = logger;

    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Email suppressed (Email:Disabled=true); would have sent to {To} — Subject: {Subject}.",
            message.To, message.Subject);
        return Task.CompletedTask;
    }

    public Task SendTemplatedAsync(string templateName, string to, object model, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Email suppressed (Email:Disabled=true); would have sent template '{Template}' to {To}.",
            templateName, to);
        return Task.CompletedTask;
    }
}
