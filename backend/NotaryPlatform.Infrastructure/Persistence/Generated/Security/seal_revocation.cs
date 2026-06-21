using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// Revocation history for seals and digital certificates.
/// </summary>
[Table("seal_revocations", Schema = "security")]
[Index("digital_certificate_id", Name = "ix_revocations_certificate_id")]
[Index("incident_id", Name = "ix_revocations_incident_id")]
[Index("revoked_at", Name = "ix_revocations_revoked_at")]
[Index("seal_id", Name = "ix_revocations_seal_id")]
[Index("tenant_id", Name = "ix_revocations_tenant_id")]
[Index("tenant_id", "revocation_code", Name = "uq_revocations_tenant_code", IsUnique = true)]
public partial class SealRevocation
{
    [Key]
    public Guid RevocationId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string RevocationCode { get; set; } = null!;

    public Guid? SealId { get; set; }

    public Guid? DigitalCertificateId { get; set; }

    public Guid? IncidentId { get; set; }

    public Guid RevokedByUserId { get; set; }

    public DateTime RevokedAt { get; set; }

    public DateTime EffectiveAt { get; set; }

    public string Reason { get; set; } = null!;

    public bool RegulatoryNotificationRequired { get; set; }

    public DateTime? RegulatoryNotifiedAt { get; set; }

    public string? RegulatoryReference { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("digital_certificate_id")]
    [InverseProperty("seal_revocations")]
    public virtual DigitalCertificate? DigitalCertificate { get; set; }

    [ForeignKey("incident_id")]
    [InverseProperty("seal_revocations")]
    public virtual SecurityIncident? Incident { get; set; }

    [ForeignKey("revoked_by_user_id")]
    [InverseProperty("seal_revocations")]
    public virtual User RevokedByUser { get; set; } = null!;

    [ForeignKey("seal_id")]
    [InverseProperty("seal_revocations")]
    public virtual Seal? Seal { get; set; }

    [InverseProperty("revocation")]
    public virtual ICollection<SealReplacement> SealReplacements { get; set; } = new List<SealReplacement>();

    [ForeignKey("tenant_id")]
    [InverseProperty("seal_revocations")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class SealRevocation
{
    public ReplacementReasonType reason_type { get; set; }
}
// </auto-enum-partial>
