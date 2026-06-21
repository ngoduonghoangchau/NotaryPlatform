using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Company-level organizational structure.
/// </summary>
[Table("organizations", Schema = "core")]
[Index("parent_organization_id", Name = "ix_organizations_parent_organization_id")]
[Index("tenant_id", Name = "ix_organizations_tenant_id")]
[Index("tenant_id", "organization_code", Name = "uq_organizations_tenant_code", IsUnique = true)]
public partial class Organization
{
    [Key]
    public Guid OrganizationId { get; set; }

    public Guid TenantId { get; set; }

    public Guid? ParentOrganizationId { get; set; }

    [StringLength(50)]
    public string OrganizationCode { get; set; } = null!;

    [StringLength(200)]
    public string OrganizationName { get; set; } = null!;

    [StringLength(250)]
    public string? LegalName { get; set; }

    [StringLength(50)]
    public string? TaxId { get; set; }

    [StringLength(100)]
    public string? RegistrationNumber { get; set; }

    [Column(TypeName = "jsonb")]
    public string Settings { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("parent_organization")]
    public virtual ICollection<Organization> InverseparentOrganization { get; set; } = new List<Organization>();

    [InverseProperty("organization")]
    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    [ForeignKey("parent_organization_id")]
    [InverseProperty("Inverseparent_organization")]
    public virtual Organization? ParentOrganization { get; set; }

    [InverseProperty("organization")]
    public virtual ICollection<Region> Regions { get; set; } = new List<Region>();

    [ForeignKey("tenant_id")]
    [InverseProperty("organizations")]
    public virtual Tenant Tenant { get; set; } = null!;

    [InverseProperty("organization")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

// <auto-enum-partial>
public partial class Organization
{
    public OrganizationType organization_type { get; set; }
    public OrganizationStatus status { get; set; }
}
// </auto-enum-partial>
