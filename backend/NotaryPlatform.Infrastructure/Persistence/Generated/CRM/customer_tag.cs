using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Tag catalog for segmentation.
/// </summary>
[Table("customer_tags", Schema = "crm")]
[Index("is_active", Name = "ix_customer_tags_is_active")]
[Index("tenant_id", Name = "ix_customer_tags_tenant_id")]
[Index("tenant_id", "tag_code", Name = "uq_customer_tags_tenant_code", IsUnique = true)]
public partial class CustomerTag
{
    [Key]
    public Guid TagId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string TagCode { get; set; } = null!;

    [StringLength(100)]
    public string TagName { get; set; } = null!;

    public string? Description { get; set; }

    [StringLength(20)]
    public string? ColorCode { get; set; }

    public bool IsSystem { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("tag")]
    public virtual ICollection<CustomerTagAssignment> CustomerTagAssignments { get; set; } = new List<CustomerTagAssignment>();

    [ForeignKey("tenant_id")]
    [InverseProperty("customer_tags")]
    public virtual Tenant Tenant { get; set; } = null!;
}
