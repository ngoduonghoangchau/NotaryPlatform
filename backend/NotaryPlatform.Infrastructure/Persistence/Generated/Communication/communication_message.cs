using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Communication;

/// <summary>
/// Individual inbound/outbound/internal messages.
/// </summary>
[Table("communication_messages", Schema = "communication")]
[Index("linked_entity_type", "linked_entity_id", Name = "ix_messages_linked_entity")]
[Index("sent_at", Name = "ix_messages_sent_at")]
[Index("tenant_id", Name = "ix_messages_tenant_id")]
[Index("thread_id", Name = "ix_messages_thread_id")]
[Index("tenant_id", "message_code", Name = "uq_messages_tenant_code", IsUnique = true)]
public partial class CommunicationMessage
{
    [Key]
    public Guid MessageId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ThreadId { get; set; }

    [StringLength(50)]
    public string MessageCode { get; set; } = null!;

    public Guid? SenderParticipantId { get; set; }

    public Guid? RecipientParticipantId { get; set; }

    public Guid? SenderUserId { get; set; }

    public Guid? SenderNotaryId { get; set; }

    [StringLength(300)]
    public string? Subject { get; set; }

    public string? BodyText { get; set; }

    public string? BodyHtml { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime? FailedAt { get; set; }

    public string? FailureReason { get; set; }

    public string? ExternalMessageId { get; set; }

    [StringLength(200)]
    public string? ProviderName { get; set; }

    [StringLength(100)]
    public string? ProviderStatus { get; set; }

    public bool IsImportant { get; set; }

    public bool IsInternal { get; set; }

    public bool IsSystemGenerated { get; set; }

    [StringLength(100)]
    public string? LinkedEntityType { get; set; }

    public Guid? LinkedEntityId { get; set; }

    [StringLength(100)]
    public string? LinkedEntityCode { get; set; }

    public Guid? ReplyToMessageId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("reply_to_message")]
    public virtual ICollection<CommunicationMessage> InversereplyToMessage { get; set; } = new List<CommunicationMessage>();

    [InverseProperty("message")]
    public virtual ICollection<CallLog> CallLogs { get; set; } = new List<CallLog>();

    [InverseProperty("message")]
    public virtual ICollection<CommunicationAttachment> CommunicationAttachments { get; set; } = new List<CommunicationAttachment>();

    [InverseProperty("message")]
    public virtual ICollection<CommunicationDeliveryLog> CommunicationDeliveryLogs { get; set; } = new List<CommunicationDeliveryLog>();

    [InverseProperty("message")]
    public virtual ICollection<CommunicationReminder> CommunicationReminders { get; set; } = new List<CommunicationReminder>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("communication_messagecreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("recipient_participant_id")]
    [InverseProperty("communication_messagerecipient_participants")]
    public virtual CommunicationParticipant? RecipientParticipant { get; set; }

    [ForeignKey("reply_to_message_id")]
    [InverseProperty("Inversereply_to_message")]
    public virtual CommunicationMessage? ReplyToMessage { get; set; }

    [ForeignKey("sender_notary_id")]
    [InverseProperty("communication_messages")]
    public virtual Notary? SenderNotary { get; set; }

    [ForeignKey("sender_participant_id")]
    [InverseProperty("communication_messagesender_participants")]
    public virtual CommunicationParticipant? SenderParticipant { get; set; }

    [ForeignKey("sender_user_id")]
    [InverseProperty("communication_messagesender_users")]
    public virtual User? SenderUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("communication_messages")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("thread_id")]
    [InverseProperty("communication_messages")]
    public virtual CommunicationThread Thread { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("communication_messageupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class CommunicationMessage
{
    public MessageDirection direction { get; set; }
    public MessageStatus message_status { get; set; }
    public ChannelType source_channel { get; set; }
}
// </auto-enum-partial>
