using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Communication;

/// <summary>
/// Scheduled reminders and notifications.
/// </summary>
[Table("communication_reminders", Schema = "communication")]
[Index("scheduled_at", Name = "ix_reminders_scheduled_at")]
[Index("tenant_id", Name = "ix_reminders_tenant_id")]
[Index("tenant_id", "reminder_code", Name = "uq_reminders_tenant_code", IsUnique = true)]
public partial class CommunicationReminder
{
    [Key]
    public Guid ReminderId { get; set; }

    public Guid TenantId { get; set; }

    public Guid? ThreadId { get; set; }

    public Guid? MessageId { get; set; }

    public Guid? JobId { get; set; }

    public Guid? NotarialActId { get; set; }

    public Guid? JournalEntryId { get; set; }

    public Guid? InvoiceId { get; set; }

    [StringLength(50)]
    public string ReminderCode { get; set; } = null!;

    public Guid? RecipientParticipantId { get; set; }

    public Guid? RecipientUserId { get; set; }

    public Guid? RecipientNotaryId { get; set; }

    [Column(TypeName = "citext")]
    public string? RecipientEmail { get; set; }

    [StringLength(30)]
    public string? RecipientPhone { get; set; }

    public Guid? TemplateId { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    public string? BodyText { get; set; }

    [Column(TypeName = "jsonb")]
    public string Payload { get; set; } = null!;

    public DateTime ScheduledAt { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? FailedAt { get; set; }

    public DateTime? SnoozedUntil { get; set; }

    public string? FailureReason { get; set; }

    public Guid? CreatedByUserId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("reminder")]
    public virtual ICollection<CallLog> CallLogs { get; set; } = new List<CallLog>();

    [InverseProperty("reminder")]
    public virtual ICollection<CommunicationDeliveryLog> CommunicationDeliveryLogs { get; set; } = new List<CommunicationDeliveryLog>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("communication_remindercreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("invoice_id")]
    [InverseProperty("communication_reminders")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("job_id")]
    [InverseProperty("communication_reminders")]
    public virtual Job? Job { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("communication_reminders")]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey("message_id")]
    [InverseProperty("communication_reminders")]
    public virtual CommunicationMessage? Message { get; set; }

    [ForeignKey("notarial_act_id")]
    [InverseProperty("communication_reminders")]
    public virtual NotarialAct? NotarialAct { get; set; }

    [ForeignKey("recipient_notary_id")]
    [InverseProperty("communication_reminders")]
    public virtual Notary? RecipientNotary { get; set; }

    [ForeignKey("recipient_participant_id")]
    [InverseProperty("communication_reminders")]
    public virtual CommunicationParticipant? RecipientParticipant { get; set; }

    [ForeignKey("recipient_user_id")]
    [InverseProperty("communication_reminderrecipient_users")]
    public virtual User? RecipientUser { get; set; }

    [ForeignKey("template_id")]
    [InverseProperty("communication_reminders")]
    public virtual CommunicationTemplate? Template { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("communication_reminders")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("thread_id")]
    [InverseProperty("communication_reminders")]
    public virtual CommunicationThread? Thread { get; set; }
}

// <auto-enum-partial>
public partial class CommunicationReminder
{
    public ChannelType channel_type { get; set; }
    public ReminderStatus reminder_status { get; set; }
}
// </auto-enum-partial>
