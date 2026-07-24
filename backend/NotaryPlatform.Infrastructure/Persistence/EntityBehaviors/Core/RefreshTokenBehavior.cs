using System.Net;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Hand-written behavior for the scaffolded <see cref="RefreshToken"/> entity, kept in a separate
/// partial so re-scaffolding never overwrites it. It encapsulates state changes so callers (e.g.
/// <c>AuthRepository</c>) mutate a token through intent-revealing methods instead of assigning
/// properties directly.
/// </summary>
public partial class RefreshToken
{
    /// <summary>Creates a new refresh-token row (SHA-256 hash only, never the raw token).</summary>
    public static RefreshToken Issue(
        Guid tenantId,
        Guid userId,
        string tokenHash,
        string? deviceName,
        string? userAgent,
        IPAddress? createdIp,
        DateTime expiresAtUtc) =>
        new()
        {
            RefreshTokenId = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            TokenHash = tokenHash,
            DeviceName = deviceName,
            UserAgent = userAgent,
            CreatedIp = createdIp,
            ExpiresAt = expiresAtUtc,
        };

    /// <summary>Revokes this token (logout, device de-duplication, or theft response).</summary>
    public void Revoke(DateTime whenUtc) => RevokedAt = whenUtc;

    /// <summary>
    /// Rotates this token out (BR-AUTH-04): revoke it, stamp last-used, and link the replacement token.
    /// </summary>
    public void RotateOut(DateTime whenUtc, Guid replacementTokenId)
    {
        RevokedAt = whenUtc;
        LastUsedAt = whenUtc;
        ReplacedByTokenId = replacementTokenId;
    }
}
