using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Customer-to-segment mapping.
/// </summary>
[PrimaryKey("customer_id", "segment_id")]
[Table("customer_segment_assignments", Schema = "crm")]
[Index("segment_id", Name = "ix_customer_segment_assignments_segment_id")]
public partial class CustomerSegmentAssignment
{
    [Key]
    public Guid CustomerId { get; set; }

    [Key]
    public Guid SegmentId { get; set; }

    public Guid? AssignedByUserId { get; set; }

    public DateTime AssignedAt { get; set; }

    [ForeignKey("assigned_by_user_id")]
    [InverseProperty("customer_segment_assignments")]
    public virtual User? AssignedByUser { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("customer_segment_assignments")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("segment_id")]
    [InverseProperty("customer_segment_assignments")]
    public virtual CustomerSegment Segment { get; set; } = null!;
}
