using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Journal.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Journal;

/// <summary>
/// Export requests and generated evidence packages.
/// </summary>
[Table("journal_exports", Schema = "journal")]
[Index("journal_entry_id", Name = "ix_journal_exports_entry_id")]
[Index("requested_at", Name = "ix_journal_exports_requested_at")]
[Index("tenant_id", Name = "ix_journal_exports_tenant_id")]
[Index("tenant_id", "export_code", Name = "uq_journal_exports_tenant_code", IsUnique = true)]
public partial class JournalExport
{
    [Key]
    public Guid JournalExportId { get; set; }

    public Guid TenantId { get; set; }

    public Guid? JournalEntryId { get; set; }

    [StringLength(50)]
    public string ExportCode { get; set; } = null!;

    [StringLength(100)]
    public string ExportScope { get; set; } = null!;

    public string? ExportReason { get; set; }

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

    public Guid RequestedByUserId { get; set; }

    public Guid? GeneratedByUserId { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? GeneratedAt { get; set; }

    public DateTime? DownloadedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("generated_by_user_id")]
    [InverseProperty("journal_exportgenerated_by_users")]
    public virtual User? GeneratedByUser { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("journal_exports")]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey("requested_by_user_id")]
    [InverseProperty("journal_exportrequested_by_users")]
    public virtual User RequestedByUser { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("journal_exports")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class JournalExport
{
    public JournalExportFormat export_format { get; set; }
    public JournalExportStatus export_status { get; set; }
}
// </auto-enum-partial>
