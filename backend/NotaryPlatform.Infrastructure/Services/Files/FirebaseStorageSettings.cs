namespace NotaryPlatform.Infrastructure.Services.Files;

/// <summary>Firebase Storage configuration — bound from appsettings.json → "FirebaseStorage" section.</summary>
public sealed class FirebaseStorageSettings
{
    public const string SectionName = "FirebaseStorage";

    /// <summary>GCS bucket name (e.g. "my-project.appspot.com").</summary>
    public required string BucketName { get; init; }

    /// <summary>Default TTL for authenticated download URLs.</summary>
    public TimeSpan DefaultDownloadUrlExpiry { get; init; } = TimeSpan.FromMinutes(15);
}
