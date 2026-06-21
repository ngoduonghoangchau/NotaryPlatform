using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Identity.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Identity;

/// <summary>
/// Status transition history for compliance and audit.
/// </summary>
[Table("notary_status_history", Schema = "identity")]
[Index("effective_at", Name = "ix_notary_status_history_effective_at")]
[Index("notary_id", Name = "ix_notary_status_history_notary_id")]
[Index("tenant_id", Name = "ix_notary_status_history_tenant_id")]
public partial class NotaryStatusHistory
{
    [Key]
    public Guid StatusHistoryId { get; set; }

    public Guid TenantId { get; set; }

    public Guid NotaryId { get; set; }

    public string? Reason { get; set; }

    public DateTime EffectiveAt { get; set; }

    public Guid? PerformedByUserId { get; set; }

    public string? SourceReference { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("notary_status_histories")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("performed_by_user_id")]
    [InverseProperty("notary_status_histories")]
    public virtual User? PerformedByUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("notary_status_histories")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class NotaryStatusHistory
{
    public HistoryActionType action_type { get; set; }
    public NotaryStatus new_status { get; set; }
    public NotaryStatus previous_status { get; set; }
}
// </auto-enum-partial>
