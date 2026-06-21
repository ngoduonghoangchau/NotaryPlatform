using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Journal.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Journal;

/// <summary>
/// Signature capture records for journal signers.
/// </summary>
[Table("journal_entry_signatures", Schema = "journal")]
[Index("signed_at", Name = "ix_journal_entry_signatures_signed_at")]
[Index("journal_entry_signer_id", Name = "ix_journal_entry_signatures_signer_id")]
[Index("tenant_id", Name = "ix_journal_entry_signatures_tenant_id")]
public partial class JournalEntrySignature
{
    [Key]
    public Guid JournalEntrySignatureId { get; set; }

    public Guid TenantId { get; set; }

    public Guid JournalEntrySignerId { get; set; }

    [StringLength(100)]
    public string? SignatureMethod { get; set; }

    [StringLength(50)]
    public string SignatureFormat { get; set; } = null!;

    [StringLength(255)]
    public string? FileName { get; set; }

    [StringLength(100)]
    public string? MimeType { get; set; }

    [StringLength(50)]
    public string StorageProvider { get; set; } = null!;

    public string? StorageKey { get; set; }

    public long? FileSizeBytes { get; set; }

    [StringLength(64)]
    public string? ChecksumSha256 { get; set; }

    public DateTime? SignedAt { get; set; }

    public Guid? CapturedByUserId { get; set; }

    [Column(TypeName = "jsonb")]
    public string DeviceInfo { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("captured_by_user_id")]
    [InverseProperty("journal_entry_signatures")]
    public virtual User? CapturedByUser { get; set; }

    [ForeignKey("journal_entry_signer_id")]
    [InverseProperty("journal_entry_signatures")]
    public virtual JournalEntrySigner JournalEntrySigner { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("journal_entry_signatures")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class JournalEntrySignature
{
    public JournalCaptureType capture_type { get; set; }
}
// </auto-enum-partial>
