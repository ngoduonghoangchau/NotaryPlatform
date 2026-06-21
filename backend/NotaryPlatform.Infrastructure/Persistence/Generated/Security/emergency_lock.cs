using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// One-click suspension / emergency lock records.
/// </summary>
[Table("emergency_locks", Schema = "security")]
[Index("digital_certificate_id", Name = "ix_emergency_locks_certificate_id")]
[Index("seal_id", Name = "ix_emergency_locks_seal_id")]
[Index("tenant_id", Name = "ix_emergency_locks_tenant_id")]
[Index("tenant_id", "lock_code", Name = "uq_emergency_locks_tenant_code", IsUnique = true)]
public partial class EmergencyLock
{
    [Key]
    public Guid EmergencyLockId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string LockCode { get; set; } = null!;

    public Guid? SealId { get; set; }

    public Guid? DigitalCertificateId { get; set; }

    public Guid? IncidentId { get; set; }

    public Guid LockedByUserId { get; set; }

    public DateTime LockedAt { get; set; }

    public Guid? ReleasedByUserId { get; set; }

    public DateTime? ReleasedAt { get; set; }

    public string LockReason { get; set; } = null!;

    public string? UnlockReason { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("digital_certificate_id")]
    [InverseProperty("emergency_locks")]
    public virtual DigitalCertificate? DigitalCertificate { get; set; }

    [ForeignKey("incident_id")]
    [InverseProperty("emergency_locks")]
    public virtual SecurityIncident? Incident { get; set; }

    [ForeignKey("locked_by_user_id")]
    [InverseProperty("emergency_locklocked_by_users")]
    public virtual User LockedByUser { get; set; } = null!;

    [ForeignKey("released_by_user_id")]
    [InverseProperty("emergency_lockreleased_by_users")]
    public virtual User? ReleasedByUser { get; set; }

    [ForeignKey("seal_id")]
    [InverseProperty("emergency_locks")]
    public virtual Seal? Seal { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("emergency_locks")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class EmergencyLock
{
    public LockStatus lock_status { get; set; }
}
// </auto-enum-partial>
