using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Refresh token/session tracking for authentication.
/// </summary>
[Table("refresh_tokens", Schema = "core")]
[Index("expires_at", Name = "ix_refresh_tokens_expires_at")]
[Index("revoked_at", Name = "ix_refresh_tokens_revoked_at")]
[Index("tenant_id", Name = "ix_refresh_tokens_tenant_id")]
[Index("user_id", Name = "ix_refresh_tokens_user_id")]
[Index("token_hash", Name = "uq_refresh_tokens_token_hash", IsUnique = true)]
public partial class RefreshToken
{
    [Key]
    public Guid RefreshTokenId { get; set; }

    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    [StringLength(64)]
    public string TokenHash { get; set; } = null!;

    [StringLength(200)]
    public string? DeviceName { get; set; }

    public string? UserAgent { get; set; }

    public IPAddress? CreatedIp { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public Guid? ReplacedByTokenId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [InverseProperty("replaced_by_token")]
    public virtual ICollection<RefreshToken> InverseReplacedByToken { get; set; } = new List<RefreshToken>();

    [ForeignKey("replaced_by_token_id")]
    [InverseProperty("Inversereplaced_by_token")]
    public virtual RefreshToken? ReplacedByToken { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("refresh_tokens")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("user_id")]
    [InverseProperty("refresh_tokens")]
    public virtual User User { get; set; } = null!;
}
