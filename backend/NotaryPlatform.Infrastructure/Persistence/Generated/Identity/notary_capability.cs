using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Identity.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Identity;

/// <summary>
/// Notary service capabilities and jurisdiction scope.
/// </summary>
[Table("notary_capabilities", Schema = "identity")]
[Index("notary_id", Name = "ix_notary_capabilities_notary_id")]
[Index("tenant_id", Name = "ix_notary_capabilities_tenant_id")]
public partial class NotaryCapability
{
    [Key]
    public Guid NotaryCapabilityId { get; set; }

    public Guid TenantId { get; set; }

    public Guid NotaryId { get; set; }

    public bool IsAuthorized { get; set; }

    [StringLength(2)]
    public string? AuthorizedStateCode { get; set; }

    [StringLength(100)]
    public string? CountyName { get; set; }

    [StringLength(20)]
    public string? ZipCode { get; set; }

    public DateOnly? ValidFrom { get; set; }

    public DateOnly? ValidTo { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("notary_capabilities")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("notary_capabilities")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class NotaryCapability
{
    public CapabilityCode capability { get; set; }
}
// </auto-enum-partial>
