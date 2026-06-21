using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// Physical seal and eSeal inventory for each notary.
/// </summary>
[Table("seals", Schema = "security")]
[Index("expires_on", Name = "ix_seals_expires_on")]
[Index("notary_id", Name = "ix_seals_notary_id")]
[Index("state_code", Name = "ix_seals_state_code")]
[Index("tenant_id", Name = "ix_seals_tenant_id")]
[Index("tenant_id", "seal_code", Name = "uq_seals_tenant_code", IsUnique = true)]
[Index("tenant_id", "seal_number", Name = "uq_seals_tenant_number", IsUnique = true)]
public partial class Seal
{
    [Key]
    public Guid SealId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string SealCode { get; set; } = null!;

    [StringLength(200)]
    public string SealName { get; set; } = null!;

    public Guid NotaryId { get; set; }

    [StringLength(2)]
    public string StateCode { get; set; } = null!;

    [StringLength(100)]
    public string? CommissionNumber { get; set; }

    [StringLength(100)]
    public string? SealNumber { get; set; }

    [StringLength(200)]
    public string? IssuingAuthority { get; set; }

    public DateOnly? EffectiveFrom { get; set; }

    public DateOnly? ExpiresOn { get; set; }

    public DateTime? IssuedAt { get; set; }

    public DateTime? SuspendedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime? LostAt { get; set; }

    public DateTime? StolenAt { get; set; }

    public DateTime? ReplacedAt { get; set; }

    public string? PhysicalDescription { get; set; }

    [StringLength(255)]
    public string? ImageFileName { get; set; }

    [StringLength(100)]
    public string? ImageMimeType { get; set; }

    [StringLength(50)]
    public string ImageStorageProvider { get; set; } = null!;

    public string? ImageStorageKey { get; set; }

    [StringLength(64)]
    public string? ImageChecksumSha256 { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public int UsageCount { get; set; }

    public bool IsCentralized { get; set; }

    public bool IsAvailableForUse { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("seal")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [InverseProperty("seal")]
    public virtual ICollection<ComplianceRule> ComplianceRules { get; set; } = new List<ComplianceRule>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("sealcreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [InverseProperty("seal")]
    public virtual ICollection<DigitalCertificate> DigitalCertificates { get; set; } = new List<DigitalCertificate>();

    [InverseProperty("seal")]
    public virtual ICollection<EmergencyLock> EmergencyLocks { get; set; } = new List<EmergencyLock>();

    [InverseProperty("seal")]
    public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    [InverseProperty("seal")]
    public virtual ICollection<LegalHold> LegalHolds { get; set; } = new List<LegalHold>();

    [ForeignKey("notary_id")]
    [InverseProperty("seals")]
    public virtual Notary Notary { get; set; } = null!;

    [InverseProperty("new_seal")]
    public virtual ICollection<SealReplacement> SealReplacementnewSeals { get; set; } = new List<SealReplacement>();

    [InverseProperty("old_seal")]
    public virtual ICollection<SealReplacement> SealReplacementoldSeals { get; set; } = new List<SealReplacement>();

    [InverseProperty("seal")]
    public virtual ICollection<SealRevocation> SealRevocations { get; set; } = new List<SealRevocation>();

    [InverseProperty("seal")]
    public virtual ICollection<SealUsageLog> SealUsageLogs { get; set; } = new List<SealUsageLog>();

    [InverseProperty("affected_seal")]
    public virtual ICollection<SecurityIncident> SecurityIncidents { get; set; } = new List<SecurityIncident>();

    [ForeignKey("tenant_id")]
    [InverseProperty("seals")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("sealupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class Seal
{
    public SealType seal_type { get; set; }
    public SealStatus status { get; set; }
}
// </auto-enum-partial>
