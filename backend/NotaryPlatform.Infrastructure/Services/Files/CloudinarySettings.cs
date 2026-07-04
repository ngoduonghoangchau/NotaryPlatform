namespace NotaryPlatform.Infrastructure.Services.Files;

/// <summary>
/// Cloudinary configuration — bound from the "Cloudinary" config section.
/// The API secret is a credential: keep it in <c>.env</c>, never in appsettings or source control.
/// </summary>
public sealed class CloudinarySettings
{
    public const string SectionName = "Cloudinary";

    public required string CloudName { get; init; }

    public required string ApiKey { get; init; }

    /// <summary>Cloudinary API secret. Treat as a secret — never log or commit.</summary>
    public required string ApiSecret { get; init; }
}
