using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

[Table("password_reset_tokens", Schema = "core")]
[Index("expires_at", Name = "ix_password_reset_tokens_expires_at")]
[Index("used_at", Name = "ix_password_reset_tokens_used_at")]
[Index("user_id", Name = "ix_password_reset_tokens_user_id")]
[Index("token_hash", Name = "uq_password_reset_tokens_token_hash", IsUnique = true)]
public partial class PasswordResetToken
{
    [Key]
    public Guid PasswordResetTokenId { get; set; }

    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    [StringLength(64)]
    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public IPAddress? CreatedIp { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("password_reset_tokencreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("password_reset_tokens")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("user_id")]
    [InverseProperty("password_reset_tokenusers")]
    public virtual User User { get; set; } = null!;
}
