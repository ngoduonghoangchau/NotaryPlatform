using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Communication;

/// <summary>
/// Conversation/thread aggregate tied to customer, job, act, invoice, or incident.
/// </summary>
[Table("communication_threads", Schema = "communication")]
[Index("customer_id", Name = "ix_threads_customer_id")]
[Index("job_id", Name = "ix_threads_job_id")]
[Index("last_activity_at", Name = "ix_threads_last_activity_at")]
[Index("notarial_act_id", Name = "ix_threads_notarial_act_id")]
[Index("tenant_id", Name = "ix_threads_tenant_id")]
[Index("tenant_id", "thread_code", Name = "uq_threads_tenant_code", IsUnique = true)]
public partial class CommunicationThread
{
    [Key]
    public Guid ThreadId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string ThreadCode { get; set; } = null!;

    [StringLength(300)]
    public string? Subject { get; set; }

    public string? Summary { get; set; }

    public bool IsInternal { get; set; }

    public bool IsImportant { get; set; }

    public bool IsPinned { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? CustomerContactId { get; set; }

    public Guid? JobId { get; set; }

    public Guid? NotarialActId { get; set; }

    public Guid? JournalEntryId { get; set; }

    public Guid? InvoiceId { get; set; }

    public Guid? PaymentId { get; set; }

    public Guid? IncidentId { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public Guid? AssignedUserId { get; set; }

    public Guid? AssignedTeamId { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public DateTime? LastActivityAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public DateTime? ArchivedAt { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("assigned_team_id")]
    [InverseProperty("communication_threads")]
    public virtual Team? AssignedTeam { get; set; }

    [ForeignKey("assigned_user_id")]
    [InverseProperty("communication_threadassigned_users")]
    public virtual User? AssignedUser { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("communication_threads")]
    public virtual Branch? Branch { get; set; }

    [InverseProperty("thread")]
    public virtual ICollection<CallLog> CallLogs { get; set; } = new List<CallLog>();

    [InverseProperty("thread")]
    public virtual ICollection<CommunicationAttachment> CommunicationAttachments { get; set; } = new List<CommunicationAttachment>();

    [InverseProperty("thread")]
    public virtual ICollection<CommunicationDeliveryLog> CommunicationDeliveryLogs { get; set; } = new List<CommunicationDeliveryLog>();

    [InverseProperty("thread")]
    public virtual ICollection<CommunicationMessage> CommunicationMessages { get; set; } = new List<CommunicationMessage>();

    [InverseProperty("thread")]
    public virtual CommunicationParticipant? CommunicationParticipant { get; set; }

    [InverseProperty("thread")]
    public virtual ICollection<CommunicationReminder> CommunicationReminders { get; set; } = new List<CommunicationReminder>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("communication_threadcreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("communication_threads")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("customer_contact_id")]
    [InverseProperty("communication_threads")]
    public virtual CustomerContact? CustomerContact { get; set; }

    [ForeignKey("incident_id")]
    [InverseProperty("communication_threads")]
    public virtual SecurityIncident? Incident { get; set; }

    [InverseProperty("thread")]
    public virtual ICollection<InternalNote> InternalNotes { get; set; } = new List<InternalNote>();

    [ForeignKey("invoice_id")]
    [InverseProperty("communication_threads")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("job_id")]
    [InverseProperty("communication_threads")]
    public virtual Job? Job { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("communication_threads")]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey("notarial_act_id")]
    [InverseProperty("communication_threads")]
    public virtual NotarialAct? NotarialAct { get; set; }

    [ForeignKey("payment_id")]
    [InverseProperty("communication_threads")]
    public virtual Payment? Payment { get; set; }

    [ForeignKey("region_id")]
    [InverseProperty("communication_threads")]
    public virtual Region? Region { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("communication_threads")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("communication_threadupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class CommunicationThread
{
    public ChannelType channel_type { get; set; }
    public ThreadStatus thread_status { get; set; }
}
// </auto-enum-partial>
