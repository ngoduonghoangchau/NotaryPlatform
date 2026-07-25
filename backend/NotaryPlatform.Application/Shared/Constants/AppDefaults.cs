namespace NotaryPlatform.Application.Shared.Constants;

/// <summary>
/// Central defaults referenced by Models and Behaviors.
/// Change values here — not in individual files.
/// </summary>
public static class AppDefaults
{
    public static class Pagination
    {
        public const int DefaultLimit = 20;
        public const int MaxLimit = 200;
        public const int DefaultPage = 1;
    }

    public static class Cache
    {
        public static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan ShortExpiry = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan LongExpiry = TimeSpan.FromHours(1);

        /// <summary>Reference / lookup data (service types, states, etc.) — rarely changes.</summary>
        public static readonly TimeSpan StaticDataExpiry = TimeSpan.FromHours(24);
    }

    public static class AsyncJob
    {
        /// <summary>How long a completed/failed job result is kept before cleanup.</summary>
        public static readonly TimeSpan ResultRetention = TimeSpan.FromDays(7);
    }

    public static class Security
    {
        public const int MaxLoginAttempts = 5;
        public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan JwtExpiry = TimeSpan.FromMinutes(60);
        public static readonly TimeSpan RefreshTokenExpiry = TimeSpan.FromDays(30);
        public static readonly TimeSpan PresignedUrlExpiry = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan PasswordResetTokenExpiry = TimeSpan.FromHours(1);   // BR-AUTH-09
    }

    public static class Files
    {
        public const long MaxUploadSizeBytes = 20 * 1024 * 1024; // 20 MB
        public static readonly string[] AllowedDocumentTypes =
            ["application/pdf", "image/jpeg", "image/png", "image/webp"];
    }
}
