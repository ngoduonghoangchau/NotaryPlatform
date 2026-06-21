using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Journal.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Journal;

/// <summary>
/// Evidence-grade audit trail for journal operations.
/// </summary>
[Table("journal_audit_logs", Schema = "journal")]
[Index("journal_entry_id", Name = "ix_journal_audit_logs_entry_id")]
[Index("occurred_at", Name = "ix_journal_audit_logs_occurred_at")]
[Index("tenant_id", Name = "ix_journal_audit_logs_tenant_id")]
public partial class JournalAuditLog
{
    [Key]
    public Guid JournalAuditLogId { get; set; }

    public Guid TenantId { get; set; }

    public Guid JournalEntryId { get; set; }

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

    [ForeignKey("actor_notary_id")]
    [InverseProperty("journal_audit_logs")]
    public virtual Notary? ActorNotary { get; set; }

    [ForeignKey("actor_user_id")]
    [InverseProperty("journal_audit_logs")]
    public virtual User? ActorUser { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("journal_audit_logs")]
    public virtual JournalEntry JournalEntry { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("journal_audit_logs")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class JournalAuditLog
{
    public JournalAuditEventType event_type { get; set; }
}
// </auto-enum-partial>
