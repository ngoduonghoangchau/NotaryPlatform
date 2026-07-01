using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NotaryPlatform.Application.Common.Interfaces;

namespace NotaryPlatform.Infrastructure.Services.Messaging;

/// <summary>
/// SMTP email sender using MailKit.
/// Implements IEmailSender (Application) — callers never reference this class directly.
///
/// LEARNING — Why MailKit over System.Net.Mail.SmtpClient?
///   The BCL SmtpClient is deprecated for async use — its async methods wrap
///   synchronous calls and still block a thread pool thread. MailKit is fully
///   async and supports STARTTLS, implicit SSL (port 465), and proper MIME
///   construction via MimeKit.
///
/// LEARNING — MimeKit BodyBuilder MIME tree:
///   TextBody only          → text/plain
///   HtmlBody only          → text/html
///   Both                   → multipart/alternative (text/plain + text/html)
///   + Attachments          → multipart/mixed wrapping the above
///   This structure is what modern email clients expect.
///
/// LEARNING — Email template strategy:
///   Templates live in EmailTemplates/{name}/subject.txt|body.html|body.txt.
///   Tokens are substituted as {{PropertyName}} → reflection over the model object.
///   For production, replace the token substitution with a Scriban template engine.
/// </summary>
public sealed class EmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<EmailSender> _logger;
    private readonly string _templateBasePath;

    public EmailSender(IOptions<SmtpSettings> options, ILogger<EmailSender> logger)
    {
        _settings = options.Value;
        _logger = logger;
        _templateBasePath = Path.Combine(AppContext.BaseDirectory, "EmailTemplates");
    }

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var mime = BuildMimeMessage(message);

        using var smtp = new SmtpClient();

        var socketOptions = _settings.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTlsWhenAvailable;

        try
        {
            await smtp.ConnectAsync(_settings.Host, _settings.Port, socketOptions, cancellationToken);
            await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, cancellationToken);
            await smtp.SendAsync(mime, cancellationToken);
        }
        finally
        {
            await smtp.DisconnectAsync(quit: true, cancellationToken);
        }

        _logger.LogInformation("Email sent to {To} — Subject: {Subject}", message.To, message.Subject);
    }

    public async Task SendTemplatedAsync(
        string templateName,
        string to,
        object model,
        CancellationToken cancellationToken = default)
    {
        var (subject, htmlBody, textBody) = await LoadTemplateAsync(templateName, model, cancellationToken);

        await SendAsync(new EmailMessage
        {
            To = to,
            Subject = subject,
            HtmlBody = htmlBody ?? string.Empty,
            TextBody = textBody,
        }, cancellationToken);
    }

    private MimeMessage BuildMimeMessage(EmailMessage message)
    {
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        mime.To.Add(MailboxAddress.Parse(message.To));

        if (!string.IsNullOrEmpty(message.Cc))
            mime.Cc.Add(MailboxAddress.Parse(message.Cc));

        if (!string.IsNullOrEmpty(message.Bcc))
            mime.Bcc.Add(MailboxAddress.Parse(message.Bcc));

        mime.Subject = message.Subject;

        var builder = new BodyBuilder
        {
            HtmlBody = message.HtmlBody,
            TextBody = message.TextBody,
        };

        if (message.Attachments is { Count: > 0 })
        {
            foreach (var a in message.Attachments)
                builder.Attachments.Add(a.FileName, a.Content, ContentType.Parse(a.ContentType));
        }

        mime.Body = builder.ToMessageBody();
        return mime;
    }

    private async Task<(string subject, string? htmlBody, string? textBody)> LoadTemplateAsync(
        string templateName,
        object model,
        CancellationToken cancellationToken)
    {
        var subjectPath = Path.Combine(_templateBasePath, templateName, "subject.txt");
        var htmlPath = Path.Combine(_templateBasePath, templateName, "body.html");
        var textPath = Path.Combine(_templateBasePath, templateName, "body.txt");

        var subject = await File.ReadAllTextAsync(subjectPath, cancellationToken);
        var htmlBody = File.Exists(htmlPath) ? await File.ReadAllTextAsync(htmlPath, cancellationToken) : null;
        var textBody = File.Exists(textPath) ? await File.ReadAllTextAsync(textPath, cancellationToken) : null;

        // Token substitution: {{PropertyName}} → value from model.
        foreach (var prop in model.GetType().GetProperties())
        {
            var token = $"{{{{{prop.Name}}}}}";
            var value = prop.GetValue(model)?.ToString() ?? string.Empty;
            subject = subject.Replace(token, value);
            htmlBody = htmlBody?.Replace(token, value);
            textBody = textBody?.Replace(token, value);
        }

        return (subject.Trim(), htmlBody, textBody);
    }
}
