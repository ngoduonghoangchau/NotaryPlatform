using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;

/// <summary>
/// Immutable status transitions for legal traceability.
/// </summary>
[Table("act_status_history", Schema = "notarial")]
[Index("act_id", Name = "ix_act_status_history_act_id")]
[Index("effective_at", Name = "ix_act_status_history_effective_at")]
[Index("tenant_id", Name = "ix_act_status_history_tenant_id")]
public partial class ActStatusHistory
{
    [Key]
    public Guid ActStatusHistoryId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ActId { get; set; }

    public string? Reason { get; set; }

    public string? SourceReference { get; set; }

    public Guid? ChangedByUserId { get; set; }

    public Guid? ChangedByNotaryId { get; set; }

    public DateTime EffectiveAt { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("act_id")]
    [InverseProperty("act_status_histories")]
    public virtual NotarialAct Act { get; set; } = null!;

    [ForeignKey("changed_by_notary_id")]
    [InverseProperty("act_status_histories")]
    public virtual Notary? ChangedByNotary { get; set; }

    [ForeignKey("changed_by_user_id")]
    [InverseProperty("act_status_histories")]
    public virtual User? ChangedByUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("act_status_histories")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class ActStatusHistory
{
    public NotarialActStatus new_status { get; set; }
    public NotarialActStatus previous_status { get; set; }
}
// </auto-enum-partial>
