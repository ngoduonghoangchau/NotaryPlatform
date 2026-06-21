using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Uploaded contract files and annexes.
/// </summary>
[Table("contract_documents", Schema = "crm")]
[Index("contract_id", Name = "ix_contract_documents_contract_id")]
[Index("document_type", Name = "ix_contract_documents_document_type")]
[Index("tenant_id", Name = "ix_contract_documents_tenant_id")]
public partial class ContractDocument
{
    [Key]
    public Guid ContractDocumentId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ContractId { get; set; }

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

    public int VersionNo { get; set; }

    public Guid UploadedByUserId { get; set; }

    public DateTime UploadedAt { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("contract_id")]
    [InverseProperty("contract_documents")]
    public virtual Contract Contract { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("contract_documents")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("uploaded_by_user_id")]
    [InverseProperty("contract_documents")]
    public virtual User UploadedByUser { get; set; } = null!;
}
