using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Status transition history for audit and compliance.
/// </summary>
[Table("customer_status_history", Schema = "crm")]
[Index("customer_id", Name = "ix_customer_status_history_customer_id")]
[Index("effective_at", Name = "ix_customer_status_history_effective_at")]
[Index("tenant_id", Name = "ix_customer_status_history_tenant_id")]
public partial class CustomerStatusHistory
{
    [Key]
    public Guid StatusHistoryId { get; set; }

    public Guid TenantId { get; set; }

    public Guid CustomerId { get; set; }

    public string? Reason { get; set; }

    public DateTime EffectiveAt { get; set; }

    public Guid? ChangedByUserId { get; set; }

    public string? SourceReference { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("changed_by_user_id")]
    [InverseProperty("customer_status_histories")]
    public virtual User? ChangedByUser { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("customer_status_histories")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("customer_status_histories")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class CustomerStatusHistory
{
    public CustomerStatus new_status { get; set; }
    public CustomerStatus previous_status { get; set; }
}
// </auto-enum-partial>
