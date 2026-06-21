using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// Digital certificates used for electronic seals and signature workflows.
/// </summary>
[Table("digital_certificates", Schema = "security")]
[Index("notary_id", Name = "ix_digital_certificates_notary_id")]
[Index("seal_id", Name = "ix_digital_certificates_seal_id")]
[Index("tenant_id", Name = "ix_digital_certificates_tenant_id")]
[Index("valid_to", Name = "ix_digital_certificates_valid_to")]
[Index("tenant_id", "serial_number", Name = "uq_digital_certificates_serial_number", IsUnique = true)]
[Index("tenant_id", "certificate_code", Name = "uq_digital_certificates_tenant_code", IsUnique = true)]
public partial class DigitalCertificate
{
    [Key]
    public Guid DigitalCertificateId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string CertificateCode { get; set; } = null!;

    [StringLength(200)]
    public string CertificateName { get; set; } = null!;

    public Guid NotaryId { get; set; }

    public Guid? SealId { get; set; }

    [StringLength(200)]
    public string ProviderName { get; set; } = null!;

    [StringLength(200)]
    public string? SubjectCommonName { get; set; }

    [StringLength(200)]
    public string? IssuerCommonName { get; set; }

    [StringLength(150)]
    public string SerialNumber { get; set; } = null!;

    [StringLength(40)]
    public string? ThumbprintSha1 { get; set; }

    [StringLength(64)]
    public string? ThumbprintSha256 { get; set; }

    [StringLength(100)]
    public string CryptographicAlgorithm { get; set; } = null!;

    [StringLength(100)]
    public string KeyStorageMethod { get; set; } = null!;

    [StringLength(100)]
    public string? HsmLabel { get; set; }

    [StringLength(100)]
    public string KeyRotationStatus { get; set; } = null!;

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public DateTime? ActivatedAt { get; set; }

    public DateTime? SuspendedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime? ReplacedAt { get; set; }

    public string? RevocationReason { get; set; }

    [Column(TypeName = "jsonb")]
    public string CertificateChain { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("certificate")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [InverseProperty("certificate")]
    public virtual ICollection<ComplianceRule> ComplianceRules { get; set; } = new List<ComplianceRule>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("digital_certificatecreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [InverseProperty("digital_certificate")]
    public virtual ICollection<DigitalCertificateChainItem> DigitalCertificateChainItems { get; set; } = new List<DigitalCertificateChainItem>();

    [InverseProperty("digital_certificate")]
    public virtual ICollection<EmergencyLock> EmergencyLocks { get; set; } = new List<EmergencyLock>();

    [InverseProperty("certificate")]
    public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    [InverseProperty("certificate")]
    public virtual ICollection<LegalHold> LegalHolds { get; set; } = new List<LegalHold>();

    [ForeignKey("notary_id")]
    [InverseProperty("digital_certificates")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("seal_id")]
    [InverseProperty("digital_certificates")]
    public virtual Seal? Seal { get; set; }

    [InverseProperty("new_certificate")]
    public virtual ICollection<SealReplacement> SealReplacementnewCertificates { get; set; } = new List<SealReplacement>();

    [InverseProperty("old_certificate")]
    public virtual ICollection<SealReplacement> SealReplacementoldCertificates { get; set; } = new List<SealReplacement>();

    [InverseProperty("digital_certificate")]
    public virtual ICollection<SealRevocation> SealRevocations { get; set; } = new List<SealRevocation>();

    [InverseProperty("digital_certificate")]
    public virtual ICollection<SealUsageLog> SealUsageLogs { get; set; } = new List<SealUsageLog>();

    [InverseProperty("affected_certificate")]
    public virtual ICollection<SecurityIncident> SecurityIncidents { get; set; } = new List<SecurityIncident>();

    [ForeignKey("tenant_id")]
    [InverseProperty("digital_certificates")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("digital_certificateupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class DigitalCertificate
{
    public CertificateStatus status { get; set; }
}
// </auto-enum-partial>
