using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Communication;

/// <summary>
/// Files attached to messages or threads.
/// </summary>
[Table("communication_attachments", Schema = "communication")]
[Index("message_id", Name = "ix_attachments_message_id")]
[Index("tenant_id", Name = "ix_attachments_tenant_id")]
[Index("thread_id", Name = "ix_attachments_thread_id")]
[Index("uploaded_at", Name = "ix_attachments_uploaded_at")]
[Index("tenant_id", "attachment_code", Name = "uq_attachments_tenant_code", IsUnique = true)]
public partial class CommunicationAttachment
{
    [Key]
    public Guid AttachmentId { get; set; }

    public Guid TenantId { get; set; }

    public Guid? MessageId { get; set; }

    public Guid ThreadId { get; set; }

    [StringLength(50)]
    public string AttachmentCode { get; set; } = null!;

    [StringLength(255)]
    public string FileName { get; set; } = null!;

    [StringLength(20)]
    public string? FileExtension { get; set; }

    [StringLength(100)]
    public string? MimeType { get; set; }

    [StringLength(50)]
    public string StorageProvider { get; set; } = null!;

    public string StorageKey { get; set; } = null!;

    public long? FileSizeBytes { get; set; }

    [StringLength(64)]
    public string? ChecksumSha256 { get; set; }

    [StringLength(300)]
    public string? Title { get; set; }

    public string? Description { get; set; }

    public bool IsSensitive { get; set; }

    [StringLength(50)]
    public string VisibilityLevel { get; set; } = null!;

    public Guid? UploadedByUserId { get; set; }

    public DateTime UploadedAt { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("message_id")]
    [InverseProperty("communication_attachments")]
    public virtual CommunicationMessage? Message { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("communication_attachments")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("thread_id")]
    [InverseProperty("communication_attachments")]
    public virtual CommunicationThread Thread { get; set; } = null!;

    [ForeignKey("uploaded_by_user_id")]
    [InverseProperty("communication_attachments")]
    public virtual User? UploadedByUser { get; set; }
}
