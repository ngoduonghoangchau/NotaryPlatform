using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// Trusted device registry for access control and step-up authentication.
/// </summary>
[Table("trusted_devices", Schema = "security")]
[Index("tenant_id", Name = "ix_trusted_devices_tenant_id")]
[Index("user_id", Name = "ix_trusted_devices_user_id")]
[Index("tenant_id", "device_fingerprint", Name = "uq_trusted_devices_fingerprint", IsUnique = true)]
[Index("tenant_id", "device_code", Name = "uq_trusted_devices_tenant_code", IsUnique = true)]
public partial class TrustedDevice
{
    [Key]
    public Guid TrustedDeviceId { get; set; }

    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    [StringLength(50)]
    public string DeviceCode { get; set; } = null!;

    [StringLength(200)]
    public string? DeviceName { get; set; }

    public string DeviceFingerprint { get; set; } = null!;

    [StringLength(100)]
    public string? Platform { get; set; }

    [StringLength(100)]
    public string? Browser { get; set; }

    [StringLength(100)]
    public string? OsName { get; set; }

    public IPAddress? IpAddress { get; set; }

    public DateTime FirstSeenAt { get; set; }

    public DateTime? LastSeenAt { get; set; }

    public DateTime? TrustedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("device")]
    public virtual ICollection<SealUsageLog> SealUsageLogs { get; set; } = new List<SealUsageLog>();

    [InverseProperty("affected_device")]
    public virtual ICollection<SecurityIncident> SecurityIncidents { get; set; } = new List<SecurityIncident>();

    [ForeignKey("tenant_id")]
    [InverseProperty("trusted_devices")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("user_id")]
    [InverseProperty("trusted_devices")]
    public virtual User User { get; set; } = null!;
}

// <auto-enum-partial>
public partial class TrustedDevice
{
    public DeviceStatus status { get; set; }
}
// </auto-enum-partial>
