namespace NotaryPlatform.Infrastructure.Caching;

/// <summary>
/// Strongly-typed settings bound from <c>appsettings.json → Cache</c>.
/// </summary>
public sealed class CacheSettings
{
    public const string SectionName = "Cache";

    /// <summary>
    /// Default TTL applied when a caller passes <c>absoluteExpiry = null</c>.
    /// Defaults to 10 minutes.
    /// </summary>
    public int DefaultExpiryMinutes { get; init; } = 10;
}
