using System.Net;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// Hand-written behavior for the scaffolded <see cref="TrustedDevice"/> entity, kept in a separate partial so
/// re-scaffolding never overwrites it. Encapsulates the trusted-device lifecycle for UC-AUTH-08 (§7.5) — the
/// repository mutates a device only through these methods, never by assigning properties directly. A device is
/// only ever created in the <c>Trusted</c> state (registration happens after a proven MFA verification).
/// </summary>
public partial class TrustedDevice
{
    /// <summary>Creates a new <b>trusted</b> device (registered right after a successful MFA verify).</summary>
    public static TrustedDevice TrustNew(
        Guid tenantId,
        Guid userId,
        string deviceCode,
        string fingerprint,
        string? deviceName,
        string? platform,
        string? browser,
        IPAddress? ip,
        DateTime whenUtc) =>
        new()
        {
            TrustedDeviceId = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            DeviceCode = deviceCode,
            DeviceFingerprint = fingerprint,
            DeviceName = Normalize(deviceName),
            Platform = Normalize(platform),
            Browser = Normalize(browser),
            IpAddress = ip,
            FirstSeenAt = whenUtc,
            LastSeenAt = whenUtc,
            TrustedAt = whenUtc,
            status = DeviceStatus.Trusted,
            Metadata = "{}",
        };

    /// <summary>Re-trusts an existing device (a fresh MFA verify restarts the BR-AUTH-08 window).</summary>
    public void Trust(DateTime whenUtc)
    {
        status = DeviceStatus.Trusted;
        TrustedAt = whenUtc;
        LastSeenAt = whenUtc;
        RevokedAt = null;
    }

    /// <summary>Refreshes the optional device hints on a re-trust (leaves a field unchanged when not supplied).</summary>
    public void RefreshHints(string? deviceName, string? platform, string? browser, IPAddress? ip)
    {
        if (!string.IsNullOrWhiteSpace(deviceName)) DeviceName = deviceName.Trim();
        if (!string.IsNullOrWhiteSpace(platform)) Platform = platform.Trim();
        if (!string.IsNullOrWhiteSpace(browser)) Browser = browser.Trim();
        if (ip is not null) IpAddress = ip;
    }

    /// <summary>Records that the device was just used to bypass the MFA challenge (audit — does NOT extend the window).</summary>
    public void StampSeen(DateTime whenUtc) => LastSeenAt = whenUtc;

    /// <summary>Soft-revokes the device — it can no longer bypass MFA until re-trusted via a new MFA verify.</summary>
    public void Revoke(DateTime whenUtc)
    {
        status = DeviceStatus.Revoked;
        RevokedAt = whenUtc;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
