using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Identity.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Identity;

/// <summary>
/// Surety bond records.
/// </summary>
[Table("notary_bonds", Schema = "identity")]
[Index("expiration_date", Name = "ix_notary_bonds_expiration_date")]
[Index("notary_id", Name = "ix_notary_bonds_notary_id")]
[Index("tenant_id", Name = "ix_notary_bonds_tenant_id")]
public partial class NotaryBond
{
    [Key]
    public Guid BondId { get; set; }

    public Guid TenantId { get; set; }

    public Guid NotaryId { get; set; }

    [StringLength(200)]
    public string ProviderName { get; set; } = null!;

    [StringLength(100)]
    public string? BondNumber { get; set; }

    [Precision(18, 2)]
    public decimal BondAmount { get; set; }

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
    [InverseProperty("notary_bonds")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("notary_bonds")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class NotaryBond
{
    public BondStatus status { get; set; }
}
// </auto-enum-partial>
