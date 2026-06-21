using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Segmentation catalog.
/// </summary>
[Table("customer_segments", Schema = "crm")]
[Index("tenant_id", Name = "ix_customer_segments_tenant_id")]
[Index("tenant_id", "segment_code", Name = "uq_customer_segments_tenant_code", IsUnique = true)]
public partial class CustomerSegment
{
    [Key]
    public Guid SegmentId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string SegmentCode { get; set; } = null!;

    [StringLength(100)]
    public string SegmentName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("segment")]
    public virtual ICollection<CustomerSegmentAssignment> CustomerSegmentAssignments { get; set; } = new List<CustomerSegmentAssignment>();

    [ForeignKey("tenant_id")]
    [InverseProperty("customer_segments")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class CustomerSegment
{
    public SegmentType segment_type { get; set; }
}
// </auto-enum-partial>
