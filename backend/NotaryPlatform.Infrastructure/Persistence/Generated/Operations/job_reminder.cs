using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Operational reminders and delivery tracking.
/// </summary>
[Table("job_reminders", Schema = "operations")]
[Index("job_id", Name = "ix_job_reminders_job_id")]
[Index("scheduled_at", Name = "ix_job_reminders_scheduled_at")]
[Index("tenant_id", Name = "ix_job_reminders_tenant_id")]
[Index("tenant_id", "reminder_code", Name = "uq_job_reminders_tenant_code", IsUnique = true)]
public partial class JobReminder
{
    [Key]
    public Guid JobReminderId { get; set; }

    public Guid TenantId { get; set; }

    public Guid JobId { get; set; }

    [StringLength(50)]
    public string ReminderCode { get; set; } = null!;

    public Guid? RecipientUserId { get; set; }

    public Guid? RecipientContactId { get; set; }

    public Guid? RecipientNotaryId { get; set; }

    [StringLength(100)]
    public string? TemplateCode { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    public string? MessageBody { get; set; }

    [Column(TypeName = "jsonb")]
    public string Payload { get; set; } = null!;

    public DateTime ScheduledAt { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? FailedAt { get; set; }

    public string? FailureReason { get; set; }

    public string? ExternalMessageId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("job_remindercreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("job_id")]
    [InverseProperty("job_reminders")]
    public virtual Job Job { get; set; } = null!;

    [ForeignKey("recipient_contact_id")]
    [InverseProperty("job_reminders")]
    public virtual CustomerContact? RecipientContact { get; set; }

    [ForeignKey("recipient_notary_id")]
    [InverseProperty("job_reminders")]
    public virtual Notary? RecipientNotary { get; set; }

    [ForeignKey("recipient_user_id")]
    [InverseProperty("job_reminderrecipient_users")]
    public virtual User? RecipientUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("job_reminders")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class JobReminder
{
    public ReminderChannel channel { get; set; }
    public ReminderRecipientType recipient_type { get; set; }
    public NotificationScope scope { get; set; }
    public ReminderStatus status { get; set; }
}
// </auto-enum-partial>
