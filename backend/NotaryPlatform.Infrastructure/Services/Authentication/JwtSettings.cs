namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>JWT configuration — bound from appsettings.json → "Jwt" section.</summary>
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int AccessTokenExpiryMinutes { get; init; } = 60;
    public int RefreshTokenExpiryDays { get; init; } = 30;
}
