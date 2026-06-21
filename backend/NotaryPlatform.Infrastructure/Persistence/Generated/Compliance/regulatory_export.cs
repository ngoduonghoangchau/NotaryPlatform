using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;

/// <summary>
/// Exports prepared for regulators or audits.
/// </summary>
[Table("regulatory_exports", Schema = "compliance")]
[Index("requested_at", Name = "ix_regulatory_exports_requested_at")]
[Index("tenant_id", Name = "ix_regulatory_exports_tenant_id")]
[Index("tenant_id", "export_code", Name = "uq_regulatory_exports_tenant_code", IsUnique = true)]
public partial class RegulatoryExport
{
    [Key]
    public Guid RegulatoryExportId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string ExportCode { get; set; } = null!;

    public string ExportScope { get; set; } = null!;

    public string? Reason { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? GeneratedAt { get; set; }

    public DateTime? DownloadedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public Guid RequestedByUserId { get; set; }

    public Guid? GeneratedByUserId { get; set; }

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

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("generated_by_user_id")]
    [InverseProperty("regulatory_exportgenerated_by_users")]
    public virtual User? GeneratedByUser { get; set; }

    [InverseProperty("related_export")]
    public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();

    [ForeignKey("requested_by_user_id")]
    [InverseProperty("regulatory_exportrequested_by_users")]
    public virtual User RequestedByUser { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("regulatory_exports")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class RegulatoryExport
{
    public ExportFormat export_format { get; set; }
    public ExportStatus export_status { get; set; }
}
// </auto-enum-partial>
