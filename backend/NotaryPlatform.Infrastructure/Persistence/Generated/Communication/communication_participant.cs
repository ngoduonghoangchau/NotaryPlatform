using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Communication;

/// <summary>
/// Participants in a communication thread.
/// </summary>
[Table("communication_participants", Schema = "communication")]
[Index("customer_id", Name = "ix_participants_customer_id")]
[Index("notary_id", Name = "ix_participants_notary_id")]
[Index("tenant_id", Name = "ix_participants_tenant_id")]
[Index("thread_id", Name = "ix_participants_thread_id")]
[Index("user_id", Name = "ix_participants_user_id")]
[Index("thread_id", "participant_code", Name = "uq_participants_thread_code", IsUnique = true)]
public partial class CommunicationParticipant
{
    [Key]
    public Guid ParticipantId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ThreadId { get; set; }

    [StringLength(50)]
    public string ParticipantCode { get; set; } = null!;

    public Guid? CustomerId { get; set; }

    public Guid? CustomerContactId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? NotaryId { get; set; }

    [StringLength(200)]
    public string? DisplayName { get; set; }

    [Column(TypeName = "citext")]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    public bool IsPrimary { get; set; }

    public bool IsActive { get; set; }

    public DateTime JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    public bool NotificationEmail { get; set; }

    public bool NotificationSms { get; set; }

    public bool NotificationInApp { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("callee_participant")]
    public virtual ICollection<CallLog> CallLogs { get; set; } = new List<CallLog>();

    [InverseProperty("recipient_participant")]
    public virtual ICollection<CommunicationMessage> CommunicationMessagerecipientParticipants { get; set; } = new List<CommunicationMessage>();

    [InverseProperty("sender_participant")]
    public virtual ICollection<CommunicationMessage> CommunicationMessagesenderParticipants { get; set; } = new List<CommunicationMessage>();

    [InverseProperty("recipient_participant")]
    public virtual ICollection<CommunicationReminder> CommunicationReminders { get; set; } = new List<CommunicationReminder>();

    [ForeignKey("customer_id")]
    [InverseProperty("communication_participants")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("customer_contact_id")]
    [InverseProperty("communication_participants")]
    public virtual CustomerContact? CustomerContact { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("communication_participants")]
    public virtual Notary? Notary { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("communication_participants")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("thread_id")]
    [InverseProperty("communication_participant")]
    public virtual CommunicationThread Thread { get; set; } = null!;

    [ForeignKey("user_id")]
    [InverseProperty("communication_participants")]
    public virtual User? User { get; set; }
}

// <auto-enum-partial>
public partial class CommunicationParticipant
{
    public ParticipantRole participant_role { get; set; }
}
// </auto-enum-partial>
