using System.Net;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Hand-written behavior for the scaffolded <see cref="PasswordResetToken"/> entity, kept in a separate
/// partial so re-scaffolding never overwrites it. Encapsulates the token lifecycle for UC-AUTH-05.
/// </summary>
public partial class PasswordResetToken
{
    /// <summary>Creates a new reset-token row (SHA-256 hash only, single-use).</summary>
    public static PasswordResetToken Issue(
        Guid tenantId,
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc,
        Guid? createdByUserId,
        IPAddress? createdIp) =>
        new()
        {
            PasswordResetTokenId = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAtUtc,
            CreatedByUserId = createdByUserId,
            CreatedIp = createdIp,
        };

    /// <summary>Marks the token as used — single-use (BR-AUTH-09).</summary>
    public void MarkUsed(DateTime whenUtc) => UsedAt = whenUtc;
}
