using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Internal notes and compliance notes.
/// </summary>
[Table("customer_notes", Schema = "crm")]
[Index("created_at", Name = "ix_customer_notes_created_at")]
[Index("customer_id", Name = "ix_customer_notes_customer_id")]
[Index("tenant_id", Name = "ix_customer_notes_tenant_id")]
public partial class CustomerNote
{
    [Key]
    public Guid NoteId { get; set; }

    public Guid TenantId { get; set; }

    public Guid CustomerId { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    public string Body { get; set; } = null!;

    public bool Pinned { get; set; }

    public bool IsComplianceNote { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("customer_notecreated_by_users")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("customer_id")]
    [InverseProperty("customer_notes")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("customer_notes")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("customer_noteupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class CustomerNote
{
    public NoteVisibility visibility { get; set; }
}
// </auto-enum-partial>
