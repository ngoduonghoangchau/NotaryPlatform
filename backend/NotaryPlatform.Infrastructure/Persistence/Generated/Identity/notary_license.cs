using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Identity.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Identity;

/// <summary>
/// License, certification, and verification records.
/// </summary>
[Table("notary_licenses", Schema = "identity")]
[Index("expiration_date", Name = "ix_notary_licenses_expiration_date")]
[Index("notary_id", Name = "ix_notary_licenses_notary_id")]
[Index("tenant_id", Name = "ix_notary_licenses_tenant_id")]
public partial class NotaryLicense
{
    [Key]
    public Guid LicenseId { get; set; }

    public Guid TenantId { get; set; }

    public Guid NotaryId { get; set; }

    [StringLength(100)]
    public string LicenseType { get; set; } = null!;

    [StringLength(100)]
    public string? LicenseNumber { get; set; }

    [StringLength(200)]
    public string? IssuingAuthority { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    [StringLength(50)]
    public string VerificationStatus { get; set; } = null!;

    public Guid? VerifiedByUserId { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? RejectionReason { get; set; }

    public Guid? SourceDocumentId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("notary_licenses")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("notary_licenses")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("verified_by_user_id")]
    [InverseProperty("notary_licenses")]
    public virtual User? VerifiedByUser { get; set; }
}

// <auto-enum-partial>
public partial class NotaryLicense
{
    public DocumentStatus status { get; set; }
}
// </auto-enum-partial>
