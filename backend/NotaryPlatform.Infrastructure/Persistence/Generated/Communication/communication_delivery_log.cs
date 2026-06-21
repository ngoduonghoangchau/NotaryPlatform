using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Communication;

/// <summary>
/// Delivery attempts and provider responses.
/// </summary>
[Table("communication_delivery_logs", Schema = "communication")]
[Index("requested_at", Name = "ix_delivery_logs_requested_at")]
[Index("tenant_id", Name = "ix_delivery_logs_tenant_id")]
[Index("thread_id", Name = "ix_delivery_logs_thread_id")]
[Index("tenant_id", "delivery_code", Name = "uq_delivery_logs_tenant_code", IsUnique = true)]
public partial class CommunicationDeliveryLog
{
    [Key]
    public Guid DeliveryLogId { get; set; }

    public Guid TenantId { get; set; }

    public Guid? ReminderId { get; set; }

    public Guid? MessageId { get; set; }

    public Guid ThreadId { get; set; }

    [StringLength(50)]
    public string DeliveryCode { get; set; } = null!;

    [StringLength(200)]
    public string? ProviderName { get; set; }

    public string? ProviderMessageId { get; set; }

    public string? RecipientAddress { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? QueuedAt { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? FailedAt { get; set; }

    public DateTime? BouncedAt { get; set; }

    public string? FailureReason { get; set; }

    [Column(TypeName = "jsonb")]
    public string ResponsePayload { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("message_id")]
    [InverseProperty("communication_delivery_logs")]
    public virtual CommunicationMessage? Message { get; set; }

    [ForeignKey("reminder_id")]
    [InverseProperty("communication_delivery_logs")]
    public virtual CommunicationReminder? Reminder { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("communication_delivery_logs")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("thread_id")]
    [InverseProperty("communication_delivery_logs")]
    public virtual CommunicationThread Thread { get; set; } = null!;
}

// <auto-enum-partial>
public partial class CommunicationDeliveryLog
{
    public ChannelType channel_type { get; set; }
    public DeliveryStatus delivery_status { get; set; }
}
// </auto-enum-partial>
