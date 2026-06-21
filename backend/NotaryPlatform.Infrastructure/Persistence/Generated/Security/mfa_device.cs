using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// MFA method registry for user authentication.
/// </summary>
[Table("mfa_devices", Schema = "security")]
[Index("tenant_id", Name = "ix_mfa_devices_tenant_id")]
[Index("user_id", Name = "ix_mfa_devices_user_id")]
[Index("tenant_id", "mfa_device_code", Name = "uq_mfa_devices_tenant_code", IsUnique = true)]
public partial class MfaDevice
{
    [Key]
    public Guid MfaDeviceId { get; set; }

    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    [StringLength(50)]
    public string MfaDeviceCode { get; set; } = null!;

    [StringLength(200)]
    public string? Label { get; set; }

    public string? SecretReference { get; set; }

    [Column(TypeName = "citext")]
    public string? PhoneOrEmail { get; set; }

    public bool IsPrimary { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("mfa_devices")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("user_id")]
    [InverseProperty("mfa_device")]
    public virtual User User { get; set; } = null!;
}

// <auto-enum-partial>
public partial class MfaDevice
{
    public MfaMethodType method_type { get; set; }
    public DeviceStatus status { get; set; }
}
// </auto-enum-partial>
