using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Customer-related documents and files.
/// </summary>
[Table("customer_documents", Schema = "crm")]
[Index("document_category", Name = "ix_customer_documents_category")]
[Index("customer_id", Name = "ix_customer_documents_customer_id")]
[Index("expiration_date", Name = "ix_customer_documents_expiration_date")]
[Index("tenant_id", Name = "ix_customer_documents_tenant_id")]
[Index("document_type", Name = "ix_customer_documents_type")]
public partial class CustomerDocument
{
    [Key]
    public Guid DocumentId { get; set; }

    public Guid TenantId { get; set; }

    public Guid CustomerId { get; set; }

    [StringLength(100)]
    public string DocumentCategory { get; set; } = null!;

    [StringLength(100)]
    public string DocumentType { get; set; } = null!;

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

    [StringLength(50)]
    public string VisibilityLevel { get; set; } = null!;

    public bool IsSensitive { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public Guid? UploadedByUserId { get; set; }

    public DateTime UploadedAt { get; set; }

    public Guid? VerifiedByUserId { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? VerificationNotes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("customer_documents")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("customer_documents")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("uploaded_by_user_id")]
    [InverseProperty("customer_documentuploaded_by_users")]
    public virtual User? UploadedByUser { get; set; }

    [ForeignKey("verified_by_user_id")]
    [InverseProperty("customer_documentverified_by_users")]
    public virtual User? VerifiedByUser { get; set; }
}

// <auto-enum-partial>
public partial class CustomerDocument
{
    public DocumentStatus status { get; set; }
}
// </auto-enum-partial>
