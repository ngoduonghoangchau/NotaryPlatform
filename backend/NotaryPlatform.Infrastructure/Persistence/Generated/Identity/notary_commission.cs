using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Identity.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Identity;

/// <summary>
/// Commission history and current commission state.
/// </summary>
[Table("notary_commissions", Schema = "identity")]
[Index("expiration_date", Name = "ix_notary_commissions_expiration_date")]
[Index("notary_id", Name = "ix_notary_commissions_notary_id")]
[Index("tenant_id", Name = "ix_notary_commissions_tenant_id")]
[Index("notary_id", "commission_number", Name = "uq_commissions_notary_number", IsUnique = true)]
public partial class NotaryCommission
{
    [Key]
    public Guid CommissionId { get; set; }

    public Guid TenantId { get; set; }

    public Guid NotaryId { get; set; }

    [StringLength(100)]
    public string CommissionNumber { get; set; } = null!;

    [StringLength(2)]
    public string CommissioningStateCode { get; set; } = null!;

    public DateOnly IssueDate { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public bool RenewalSubmitted { get; set; }

    public DateTime? RenewalSubmittedAt { get; set; }

    public DateOnly? ExpectedRenewalDate { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevokedReason { get; set; }

    public Guid? SourceDocumentId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("notary_commissions")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("notary_commissions")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class NotaryCommission
{
    public CommissionStatus status { get; set; }
}
// </auto-enum-partial>
