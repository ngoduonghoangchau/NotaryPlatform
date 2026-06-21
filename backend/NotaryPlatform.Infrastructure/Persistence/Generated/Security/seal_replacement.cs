using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// Replacement history for seals and certificates.
/// </summary>
[Table("seal_replacements", Schema = "security")]
[Index("issued_at", Name = "ix_replacements_issued_at")]
[Index("new_certificate_id", Name = "ix_replacements_new_certificate_id")]
[Index("new_seal_id", Name = "ix_replacements_new_seal_id")]
[Index("old_certificate_id", Name = "ix_replacements_old_certificate_id")]
[Index("old_seal_id", Name = "ix_replacements_old_seal_id")]
[Index("tenant_id", Name = "ix_replacements_tenant_id")]
[Index("tenant_id", "replacement_code", Name = "uq_replacements_tenant_code", IsUnique = true)]
public partial class SealReplacement
{
    [Key]
    public Guid ReplacementId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string ReplacementCode { get; set; } = null!;

    public Guid? OldSealId { get; set; }

    public Guid? OldCertificateId { get; set; }

    public Guid? NewSealId { get; set; }

    public Guid? NewCertificateId { get; set; }

    public Guid? IncidentId { get; set; }

    public Guid? RevocationId { get; set; }

    public string Reason { get; set; } = null!;

    public Guid IssuedByUserId { get; set; }

    public DateTime IssuedAt { get; set; }

    public DateTime EffectiveAt { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("incident_id")]
    [InverseProperty("seal_replacements")]
    public virtual SecurityIncident? Incident { get; set; }

    [ForeignKey("issued_by_user_id")]
    [InverseProperty("seal_replacements")]
    public virtual User IssuedByUser { get; set; } = null!;

    [ForeignKey("new_certificate_id")]
    [InverseProperty("seal_replacementnew_certificates")]
    public virtual DigitalCertificate? NewCertificate { get; set; }

    [ForeignKey("new_seal_id")]
    [InverseProperty("seal_replacementnew_seals")]
    public virtual Seal? NewSeal { get; set; }

    [ForeignKey("old_certificate_id")]
    [InverseProperty("seal_replacementold_certificates")]
    public virtual DigitalCertificate? OldCertificate { get; set; }

    [ForeignKey("old_seal_id")]
    [InverseProperty("seal_replacementold_seals")]
    public virtual Seal? OldSeal { get; set; }

    [ForeignKey("revocation_id")]
    [InverseProperty("seal_replacements")]
    public virtual SealRevocation? Revocation { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("seal_replacements")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class SealReplacement
{
    public ReplacementReasonType reason_type { get; set; }
}
// </auto-enum-partial>
