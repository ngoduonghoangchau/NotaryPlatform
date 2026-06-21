using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Daily operational dashboard aggregates.
/// </summary>
[Table("daily_operational_snapshots", Schema = "operations")]
[Index("snapshot_date", Name = "ix_daily_operational_snapshots_snapshot_date")]
[Index("tenant_id", Name = "ix_daily_operational_snapshots_tenant_id")]
[Index("tenant_id", "snapshot_date", "branch_id", "region_id", Name = "uq_daily_operational_snapshots", IsUnique = true)]
public partial class DailyOperationalSnapshot
{
    [Key]
    public Guid DailySnapshotId { get; set; }

    public Guid TenantId { get; set; }

    public DateOnly SnapshotDate { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public int TotalRequests { get; set; }

    public int TotalJobs { get; set; }

    public int CompletedJobs { get; set; }

    public int CancelledJobs { get; set; }

    public int FailedJobs { get; set; }

    public int OnTimeJobs { get; set; }

    public int ReassignedJobs { get; set; }

    public int TotalRemindersSent { get; set; }

    public int TotalDispatchRuns { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metrics { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("daily_operational_snapshots")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("region_id")]
    [InverseProperty("daily_operational_snapshots")]
    public virtual Region? Region { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("daily_operational_snapshots")]
    public virtual Tenant Tenant { get; set; } = null!;
}
