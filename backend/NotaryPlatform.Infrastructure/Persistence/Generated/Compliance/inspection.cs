using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;

/// <summary>
/// Regulatory inspection tracking.
/// </summary>
[Table("inspections", Schema = "compliance")]
[Index("started_at", Name = "ix_inspections_started_at")]
[Index("state_code", Name = "ix_inspections_state_code")]
[Index("tenant_id", Name = "ix_inspections_tenant_id")]
[Index("tenant_id", "inspection_code", Name = "uq_inspections_tenant_code", IsUnique = true)]
public partial class Inspection
{
    [Key]
    public Guid InspectionId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string InspectionCode { get; set; } = null!;

    [StringLength(200)]
    public string InspectionName { get; set; } = null!;

    [StringLength(2)]
    public string? StateCode { get; set; }

    [StringLength(200)]
    public string? AuthorityName { get; set; }

    [StringLength(100)]
    public string? CaseNumber { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? DueAt { get; set; }

    [StringLength(200)]
    public string? InspectorName { get; set; }

    [Column(TypeName = "citext")]
    public string? InspectorEmail { get; set; }

    [StringLength(30)]
    public string? InspectorPhone { get; set; }

    public string? ScopeSummary { get; set; }

    public string? FindingsSummary { get; set; }

    public string? RemediationSummary { get; set; }

    public string? Outcome { get; set; }

    public Guid? RelatedExportId { get; set; }

    public Guid? LegalHoldId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("inspectioncreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("legal_hold_id")]
    [InverseProperty("inspections")]
    public virtual LegalHold? LegalHold { get; set; }

    [ForeignKey("related_export_id")]
    [InverseProperty("inspections")]
    public virtual RegulatoryExport? RelatedExport { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("inspections")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("inspectionupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class Inspection
{
    public InspectionStatus inspection_status { get; set; }
}
// </auto-enum-partial>
