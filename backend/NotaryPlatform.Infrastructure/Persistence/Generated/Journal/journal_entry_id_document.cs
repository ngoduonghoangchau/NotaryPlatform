using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Journal;

/// <summary>
/// ID scans and identity evidence for journal signers.
/// </summary>
[Table("journal_entry_id_documents", Schema = "journal")]
[Index("captured_at", Name = "ix_journal_entry_id_documents_captured_at")]
[Index("journal_entry_signer_id", Name = "ix_journal_entry_id_documents_signer_id")]
[Index("tenant_id", Name = "ix_journal_entry_id_documents_tenant_id")]
public partial class JournalEntryIdDocument
{
    [Key]
    public Guid JournalEntryIdDocumentId { get; set; }

    public Guid TenantId { get; set; }

    public Guid JournalEntrySignerId { get; set; }

    [StringLength(100)]
    public string DocumentType { get; set; } = null!;

    [StringLength(20)]
    public string? Side { get; set; }

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

    public DateTime CapturedAt { get; set; }

    public Guid? CapturedByUserId { get; set; }

    [StringLength(50)]
    public string VisibilityLevel { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("captured_by_user_id")]
    [InverseProperty("journal_entry_id_documents")]
    public virtual User? CapturedByUser { get; set; }

    [ForeignKey("journal_entry_signer_id")]
    [InverseProperty("journal_entry_id_documents")]
    public virtual JournalEntrySigner JournalEntrySigner { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("journal_entry_id_documents")]
    public virtual Tenant Tenant { get; set; } = null!;
}
