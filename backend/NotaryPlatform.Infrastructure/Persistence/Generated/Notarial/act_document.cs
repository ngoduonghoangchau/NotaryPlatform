using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;

/// <summary>
/// Supporting documents and uploaded evidence for an act.
/// </summary>
[Table("act_documents", Schema = "notarial")]
[Index("act_id", Name = "ix_act_documents_act_id")]
[Index("tenant_id", Name = "ix_act_documents_tenant_id")]
[Index("uploaded_at", Name = "ix_act_documents_uploaded_at")]
public partial class ActDocument
{
    [Key]
    public Guid ActDocumentId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ActId { get; set; }

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

    [ForeignKey("act_id")]
    [InverseProperty("act_documents")]
    public virtual NotarialAct Act { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("act_documents")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("uploaded_by_user_id")]
    [InverseProperty("act_documents")]
    public virtual User? UploadedByUser { get; set; }
}

// <auto-enum-partial>
public partial class ActDocument
{
    public DocumentLinkType document_link_type { get; set; }
}
// </auto-enum-partial>
