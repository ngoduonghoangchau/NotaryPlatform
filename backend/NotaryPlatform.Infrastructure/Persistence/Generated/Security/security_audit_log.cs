using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// Evidence-grade audit trail for security operations.
/// </summary>
[Table("security_audit_logs", Schema = "security")]
[Index("entity_type", "entity_id", Name = "ix_security_audit_logs_entity")]
[Index("occurred_at", Name = "ix_security_audit_logs_occurred_at")]
[Index("tenant_id", Name = "ix_security_audit_logs_tenant_id")]
[Index("tenant_id", "audit_code", Name = "uq_security_audit_logs_tenant_code", IsUnique = true)]
public partial class SecurityAuditLog
{
    [Key]
    public Guid SecurityAuditLogId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string AuditCode { get; set; } = null!;

    [StringLength(100)]
    public string EntityType { get; set; } = null!;

    public Guid? EntityId { get; set; }

    [StringLength(100)]
    public string? EntityCode { get; set; }

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

    [ForeignKey("actor_notary_id")]
    [InverseProperty("security_audit_logs")]
    public virtual Notary? ActorNotary { get; set; }

    [ForeignKey("actor_user_id")]
    [InverseProperty("security_audit_logs")]
    public virtual User? ActorUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("security_audit_logs")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class SecurityAuditLog
{
    public AuditEventType event_type { get; set; }
}
// </auto-enum-partial>
