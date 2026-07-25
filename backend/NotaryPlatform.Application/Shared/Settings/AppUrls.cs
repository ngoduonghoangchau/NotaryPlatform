namespace NotaryPlatform.Application.Shared.Settings;

/// <summary>
/// Public-facing URLs (bound from the <c>App</c> config section). Used to build links that appear in
/// outbound email — e.g. the password-reset link points at the frontend reset page, not the API.
/// </summary>
public sealed class AppUrls
{
    public const string SectionName = "App";

    /// <summary>Public base URL of the frontend (e.g. <c>https://app.example.com</c>).</summary>
    public string PublicBaseUrl { get; init; } = "https://localhost";
}
