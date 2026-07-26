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

        // ── MFA (UC-AUTH-06) ──────────────────────────────────────────────
        /// <summary>How many single-use recovery codes are issued when a TOTP device is verified (D-3).</summary>
        public const int RecoveryCodeCount = 10;

        /// <summary>Max failed MFA-verify attempts before a temporary per-user lockout (brute-force guard).</summary>
        public const int MaxMfaVerifyAttempts = 5;

        /// <summary>How long the MFA-verify lockout lasts (auto-lifts via the cache TTL).</summary>
        public static readonly TimeSpan MfaVerifyLockoutDuration = TimeSpan.FromMinutes(15);

        /// <summary>How long an MFA login challenge (UC-AUTH-07) is valid — short, single-use.</summary>
        public static readonly TimeSpan MfaChallengeTtl = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Maximum trusted-device MFA-bypass window (UC-AUTH-08, BR-AUTH-08). A trusted device skips the MFA
        /// challenge only while <c>trusted_at + TrustedDevicePeriod &gt; now</c>; after that, MFA is required again.
        /// </summary>
        public static readonly TimeSpan TrustedDevicePeriod = TimeSpan.FromDays(30);

        /// <summary>
        /// Allowed device-fingerprint format (UC-AUTH-08, S-1) — a high-entropy client identifier, 8–200 chars
        /// from an unambiguous charset. Single-sourced here so every validator uses the same rule.
        /// </summary>
        public const string DeviceFingerprintPattern = "^[A-Za-z0-9._:-]{8,200}$";
    }

    public static class Files
    {
        public const long MaxUploadSizeBytes = 20 * 1024 * 1024; // 20 MB
        public static readonly string[] AllowedDocumentTypes =
            ["application/pdf", "image/jpeg", "image/png", "image/webp"];
    }
}
