using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;

/// <summary>
/// Evidence-grade audit log for every significant act change.
/// </summary>
[Table("act_audit_logs", Schema = "notarial")]
[Index("act_id", Name = "ix_act_audit_logs_act_id")]
[Index("occurred_at", Name = "ix_act_audit_logs_occurred_at")]
[Index("tenant_id", Name = "ix_act_audit_logs_tenant_id")]
public partial class ActAuditLog
{
    [Key]
    public Guid AuditLogId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ActId { get; set; }

    [StringLength(200)]
    public string? EventTitle { get; set; }

    public string? EventBody { get; set; }

    public Guid? ActorUserId { get; set; }

    public Guid? ActorNotaryId { get; set; }

    public IPAddress? SourceIp { get; set; }

    public string? UserAgent { get; set; }

    [Column(TypeName = "jsonb")]
    public string? BeforeData { get; set; }

    [Column(TypeName = "jsonb")]
    public string? AfterData { get; set; }

    public DateTime OccurredAt { get; set; }

    public string? SourceReference { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("act_id")]
    [InverseProperty("act_audit_logs")]
    public virtual NotarialAct Act { get; set; } = null!;

    [ForeignKey("actor_notary_id")]
    [InverseProperty("act_audit_logs")]
    public virtual Notary? ActorNotary { get; set; }

    [ForeignKey("actor_user_id")]
    [InverseProperty("act_audit_logs")]
    public virtual User? ActorUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("act_audit_logs")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class ActAuditLog
{
    public ActEventType event_type { get; set; }
}
// </auto-enum-partial>
