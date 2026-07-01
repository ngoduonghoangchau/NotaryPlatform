namespace NotaryPlatform.Infrastructure.Services.Messaging;

/// <summary>SMTP configuration — bound from appsettings.json → "Smtp" section.</summary>
public sealed class SmtpSettings
{
    public const string SectionName = "Smtp";

    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
    public required string SenderEmail { get; init; }
    public required string SenderName { get; init; }
    /// <summary>
    /// true  = implicit TLS from the first byte (port 465, SslOnConnect).
    /// false = STARTTLS upgrade on a plaintext connection (port 587, StartTlsWhenAvailable).
    /// </summary>
    public bool UseSsl { get; init; } = false;
}
