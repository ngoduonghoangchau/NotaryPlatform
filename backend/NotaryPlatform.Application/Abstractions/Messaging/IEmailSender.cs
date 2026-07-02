namespace NotaryPlatform.Application.Abstractions.Messaging;

/// <summary>
/// Abstracts outbound email. Implemented by Infrastructure.Services.Messaging.EmailSender (MailKit).
/// </summary>
public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
    Task SendTemplatedAsync(string templateName, string to, object model, CancellationToken cancellationToken = default);
}

public sealed record EmailMessage
{
    public required string To { get; init; }
    public string? Cc { get; init; }
    public string? Bcc { get; init; }
    public required string Subject { get; init; }
    public required string HtmlBody { get; init; }
    public string? TextBody { get; init; }
    public IReadOnlyList<EmailAttachment>? Attachments { get; init; }
}

public sealed record EmailAttachment
{
    public required string FileName { get; init; }
    public required byte[] Content { get; init; }
    public required string ContentType { get; init; }
}
