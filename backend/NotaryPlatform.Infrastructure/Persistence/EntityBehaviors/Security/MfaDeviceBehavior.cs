using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// Hand-written behavior for the scaffolded <see cref="MfaDevice"/> entity, kept in a separate partial so
/// re-scaffolding never overwrites it. Encapsulates the MFA device lifecycle for UC-AUTH-06 (§7.5) — the
/// repository mutates a device only through these methods, never by assigning properties directly.
/// </summary>
public partial class MfaDevice
{
    /// <summary>Creates a new <b>pending</b> TOTP device (unverified until a valid code proves it — Step A).</summary>
    public static MfaDevice EnrollTotp(
        Guid tenantId,
        Guid userId,
        string deviceCode,
        string? label,
        string secretReference) =>
        new()
        {
            MfaDeviceId = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            MfaDeviceCode = deviceCode,
            Label = label,
            SecretReference = secretReference,
            method_type = MfaMethodType.Totp,
            status = DeviceStatus.Pending,
            IsPrimary = false,
            IsVerified = false,
            Metadata = "{}",
        };

    /// <summary>
    /// Creates a <c>recovery_code</c> device row holding the hashed codes in <paramref name="metadataJson"/>
    /// (D-3). Active on creation (issued alongside a verified authenticator) but never primary.
    /// </summary>
    public static MfaDevice CreateRecoveryCodeSet(
        Guid tenantId,
        Guid userId,
        string deviceCode,
        string metadataJson) =>
        new()
        {
            MfaDeviceId = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            MfaDeviceCode = deviceCode,
            method_type = MfaMethodType.RecoveryCode,
            status = DeviceStatus.Trusted,
            IsPrimary = false,
            IsVerified = true,
            Metadata = metadataJson,
        };

    /// <summary>Marks the device verified and primary — Step B success (satisfies BR-AUTH-05).</summary>
    public void MarkVerifiedPrimary(DateTime whenUtc)
    {
        IsVerified = true;
        VerifiedAt = whenUtc;
        status = DeviceStatus.Trusted;
        IsPrimary = true;
    }

    /// <summary>Revokes the device and clears its primary flag (supersede on re-enrollment — D-5).</summary>
    public void Revoke(DateTime whenUtc)
    {
        RevokedAt = whenUtc;
        IsPrimary = false;
        status = DeviceStatus.Revoked;
    }

    /// <summary>Expires a still-pending enrollment that a fresh enrollment supersedes (one in-flight at a time).</summary>
    public void ExpirePending(DateTime whenUtc)
    {
        RevokedAt = whenUtc;
        IsPrimary = false;
        status = DeviceStatus.Expired;
    }
}
