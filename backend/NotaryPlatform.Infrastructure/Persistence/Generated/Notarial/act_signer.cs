using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;

/// <summary>
/// One or more signers associated with a notarial act.
/// </summary>
[Table("act_signers", Schema = "notarial")]
[Index("act_id", Name = "ix_act_signers_act_id")]
[Index("full_legal_name", Name = "ix_act_signers_full_legal_name")]
[Index("is_primary_signer", Name = "ix_act_signers_is_primary_signer")]
[Index("tenant_id", Name = "ix_act_signers_tenant_id")]
public partial class ActSigner
{
    [Key]
    public Guid ActSignerId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ActId { get; set; }

    public int SignerOrder { get; set; }

    [StringLength(200)]
    public string FullLegalName { get; set; } = null!;

    [StringLength(150)]
    public string? CapacityOrTitle { get; set; }

    public bool IsPrincipal { get; set; }

    public bool IsPrimarySigner { get; set; }

    [Column(TypeName = "citext")]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? AddressLine1 { get; set; }

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(2)]
    public string? CountryCode { get; set; }

    [StringLength(100)]
    public string? IdType { get; set; }

    [StringLength(100)]
    public string? IdNumber { get; set; }

    [StringLength(200)]
    public string? IssuingAuthority { get; set; }

    public DateOnly? IdIssueDate { get; set; }

    public DateOnly? IdExpirationDate { get; set; }

    public bool AppearanceConfirmed { get; set; }

    public bool SignatureRequired { get; set; }

    public bool SignatureCaptured { get; set; }

    public DateTime? SignatureCapturedAt { get; set; }

    public bool ThumbprintRequired { get; set; }

    public bool ThumbprintCaptured { get; set; }

    public DateTime? ThumbprintCapturedAt { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("act_id")]
    [InverseProperty("act_signer")]
    public virtual NotarialAct Act { get; set; } = null!;

    [InverseProperty("act_signer")]
    public virtual ICollection<ActIdentityVerification> ActIdentityVerifications { get; set; } = new List<ActIdentityVerification>();

    [ForeignKey("tenant_id")]
    [InverseProperty("act_signers")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class ActSigner
{
    public AppearanceType appearance_type { get; set; }
    public SignerRole signer_role { get; set; }
}
// </auto-enum-partial>
