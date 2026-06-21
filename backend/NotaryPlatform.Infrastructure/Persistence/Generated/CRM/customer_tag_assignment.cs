using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Customer-to-tag mapping.
/// </summary>
[PrimaryKey("customer_id", "tag_id")]
[Table("customer_tag_assignments", Schema = "crm")]
[Index("tag_id", Name = "ix_customer_tag_assignments_tag_id")]
public partial class CustomerTagAssignment
{
    [Key]
    public Guid CustomerId { get; set; }

    [Key]
    public Guid TagId { get; set; }

    public Guid? AssignedByUserId { get; set; }

    public DateTime AssignedAt { get; set; }

    [ForeignKey("assigned_by_user_id")]
    [InverseProperty("customer_tag_assignments")]
    public virtual User? AssignedByUser { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("customer_tag_assignments")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("tag_id")]
    [InverseProperty("customer_tag_assignments")]
    public virtual CustomerTag Tag { get; set; } = null!;
}
