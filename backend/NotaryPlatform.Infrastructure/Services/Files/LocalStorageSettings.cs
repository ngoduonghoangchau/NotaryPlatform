namespace NotaryPlatform.Infrastructure.Services.Files;

/// <summary>Local filesystem storage configuration — bound from appsettings.json → "LocalStorage" section.</summary>
public sealed class LocalStorageSettings
{
    public const string SectionName = "LocalStorage";

    /// <summary>
    /// Sub-path under wwwroot where uploaded files are stored.
    /// Served by UseStaticFiles() middleware as /uploads/{path}.
    /// </summary>
    public string UploadPath { get; init; } = "uploads";

    /// <summary>Base URL used to build public download URLs (e.g. "https://localhost:5001").</summary>
    public string BaseUrl { get; init; } = "https://localhost:5001";
}
