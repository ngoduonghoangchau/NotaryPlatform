using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;

/// <summary>
/// Identity verification steps and evidence for signers.
/// </summary>
[Table("act_identity_verifications", Schema = "notarial")]
[Index("act_id", Name = "ix_act_identity_verifications_act_id")]
[Index("act_signer_id", Name = "ix_act_identity_verifications_act_signer_id")]
[Index("tenant_id", Name = "ix_act_identity_verifications_tenant_id")]
public partial class ActIdentityVerification
{
    [Key]
    public Guid VerificationId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ActId { get; set; }

    public Guid ActSignerId { get; set; }

    public Guid? VerifiedByUserId { get; set; }

    public Guid? VerifiedByNotaryId { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public int KbaAttemptCount { get; set; }

    [Precision(5, 2)]
    public decimal? KbaScore { get; set; }

    [Precision(5, 2)]
    public decimal? CredentialAnalysisScore { get; set; }

    [StringLength(100)]
    public string? IdType { get; set; }

    [StringLength(100)]
    public string? IdNumber { get; set; }

    [StringLength(200)]
    public string? IssuingAuthority { get; set; }

    public DateOnly? IdIssueDate { get; set; }

    public DateOnly? IdExpirationDate { get; set; }

    public Guid? IdFrontFileId { get; set; }

    public Guid? IdBackFileId { get; set; }

    public Guid? SelfieFileId { get; set; }

    public string? FailureReason { get; set; }

    public string? ReviewNotes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("act_id")]
    [InverseProperty("act_identity_verifications")]
    public virtual NotarialAct Act { get; set; } = null!;

    [ForeignKey("act_signer_id")]
    [InverseProperty("act_identity_verifications")]
    public virtual ActSigner ActSigner { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("act_identity_verifications")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("verified_by_notary_id")]
    [InverseProperty("act_identity_verifications")]
    public virtual Notary? VerifiedByNotary { get; set; }

    [ForeignKey("verified_by_user_id")]
    [InverseProperty("act_identity_verifications")]
    public virtual User? VerifiedByUser { get; set; }
}

// <auto-enum-partial>
public partial class ActIdentityVerification
{
    public VerificationResult result { get; set; }
    public IdentityVerificationMethod verification_method { get; set; }
}
// </auto-enum-partial>
