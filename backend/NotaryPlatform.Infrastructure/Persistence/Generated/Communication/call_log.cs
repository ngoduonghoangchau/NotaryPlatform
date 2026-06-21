using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Communication;

/// <summary>
/// Call records, outcomes, and optional recordings.
/// </summary>
[Table("call_logs", Schema = "communication")]
[Index("started_at", Name = "ix_call_logs_started_at")]
[Index("tenant_id", Name = "ix_call_logs_tenant_id")]
[Index("thread_id", Name = "ix_call_logs_thread_id")]
[Index("tenant_id", "call_code", Name = "uq_call_logs_tenant_code", IsUnique = true)]
public partial class CallLog
{
    [Key]
    public Guid CallLogId { get; set; }

    public Guid TenantId { get; set; }

    public Guid? ThreadId { get; set; }

    public Guid? ReminderId { get; set; }

    public Guid? MessageId { get; set; }

    [StringLength(50)]
    public string CallCode { get; set; } = null!;

    public Guid? CallerUserId { get; set; }

    public Guid? CallerNotaryId { get; set; }

    public Guid? CalleeParticipantId { get; set; }

    public Guid? CalleeUserId { get; set; }

    public Guid? CalleeNotaryId { get; set; }

    [StringLength(30)]
    public string? CalleePhone { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public int? DurationSeconds { get; set; }

    public string? Summary { get; set; }

    public string? Notes { get; set; }

    [StringLength(255)]
    public string? RecordingFileName { get; set; }

    public string? RecordingStorageKey { get; set; }

    [StringLength(100)]
    public string? RecordingMimeType { get; set; }

    public long? RecordingFileSizeBytes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("callee_notary_id")]
    [InverseProperty("call_logcallee_notaries")]
    public virtual Notary? CalleeNotary { get; set; }

    [ForeignKey("callee_participant_id")]
    [InverseProperty("call_logs")]
    public virtual CommunicationParticipant? CalleeParticipant { get; set; }

    [ForeignKey("callee_user_id")]
    [InverseProperty("call_logcallee_users")]
    public virtual User? CalleeUser { get; set; }

    [ForeignKey("caller_notary_id")]
    [InverseProperty("call_logcaller_notaries")]
    public virtual Notary? CallerNotary { get; set; }

    [ForeignKey("caller_user_id")]
    [InverseProperty("call_logcaller_users")]
    public virtual User? CallerUser { get; set; }

    [ForeignKey("message_id")]
    [InverseProperty("call_logs")]
    public virtual CommunicationMessage? Message { get; set; }

    [ForeignKey("reminder_id")]
    [InverseProperty("call_logs")]
    public virtual CommunicationReminder? Reminder { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("call_logs")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("thread_id")]
    [InverseProperty("call_logs")]
    public virtual CommunicationThread? Thread { get; set; }
}

// <auto-enum-partial>
public partial class CallLog
{
    public CallOutcome call_outcome { get; set; }
    public CallStatus call_status { get; set; }
}
// </auto-enum-partial>
