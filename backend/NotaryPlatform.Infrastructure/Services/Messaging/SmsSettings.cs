namespace NotaryPlatform.Infrastructure.Services.Messaging;

/// <summary>SMS (Twilio) configuration — bound from appsettings.json → "Sms" section.</summary>
public sealed class SmsSettings
{
    public const string SectionName = "Sms";

    /// <summary>Twilio Account SID (starts with "AC…").</summary>
    public required string AccountSid { get; init; }

    /// <summary>Twilio Auth Token. Treat as a secret — never log or commit.</summary>
    public required string AuthToken { get; init; }

    /// <summary>Sender phone number in E.164 format, e.g. "+84901234567".</summary>
    public required string From { get; init; }
}
