using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Identity.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Identity;

/// <summary>
/// Errors and Omissions insurance records.
/// </summary>
[Table("notary_insurances", Schema = "identity")]
[Index("expiration_date", Name = "ix_notary_insurances_expiration_date")]
[Index("notary_id", Name = "ix_notary_insurances_notary_id")]
[Index("tenant_id", Name = "ix_notary_insurances_tenant_id")]
public partial class NotaryInsurance
{
    [Key]
    public Guid InsuranceId { get; set; }

    public Guid TenantId { get; set; }

    public Guid NotaryId { get; set; }

    [StringLength(200)]
    public string ProviderName { get; set; } = null!;

    [StringLength(100)]
    public string? PolicyNumber { get; set; }

    [Precision(18, 2)]
    public decimal CoverageAmount { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public Guid? DocumentFileId { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("notary_insurances")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("notary_insurances")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class NotaryInsurance
{
    public InsuranceStatus status { get; set; }
}
// </auto-enum-partial>
