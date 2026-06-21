using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Journal;

/// <summary>
/// Thumbprint capture records for journal signers.
/// </summary>
[Table("journal_entry_thumbprints", Schema = "journal")]
[Index("captured_at", Name = "ix_journal_entry_thumbprints_captured_at")]
[Index("journal_entry_signer_id", Name = "ix_journal_entry_thumbprints_signer_id")]
[Index("tenant_id", Name = "ix_journal_entry_thumbprints_tenant_id")]
public partial class JournalEntryThumbprint
{
    [Key]
    public Guid JournalEntryThumbprintId { get; set; }

    public Guid TenantId { get; set; }

    public Guid JournalEntrySignerId { get; set; }

    [StringLength(100)]
    public string? ThumbprintMethod { get; set; }

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

    public DateTime? CapturedAt { get; set; }

    public Guid? CapturedByUserId { get; set; }

    [Column(TypeName = "jsonb")]
    public string DeviceInfo { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("captured_by_user_id")]
    [InverseProperty("journal_entry_thumbprints")]
    public virtual User? CapturedByUser { get; set; }

    [ForeignKey("journal_entry_signer_id")]
    [InverseProperty("journal_entry_thumbprints")]
    public virtual JournalEntrySigner JournalEntrySigner { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("journal_entry_thumbprints")]
    public virtual Tenant Tenant { get; set; } = null!;
}
