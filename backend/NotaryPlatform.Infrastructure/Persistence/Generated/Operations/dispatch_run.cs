using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Dispatch evaluation runs and algorithm snapshots.
/// </summary>
[Table("dispatch_runs", Schema = "operations")]
[Index("job_request_id", Name = "ix_dispatch_runs_job_request_id")]
[Index("tenant_id", Name = "ix_dispatch_runs_tenant_id")]
[Index("tenant_id", "run_code", Name = "uq_dispatch_runs_tenant_code", IsUnique = true)]
public partial class DispatchRun
{
    [Key]
    public Guid DispatchRunId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string RunCode { get; set; } = null!;

    public Guid JobRequestId { get; set; }

    [StringLength(100)]
    public string? AlgorithmName { get; set; }

    [StringLength(50)]
    public string? AlgorithmVersion { get; set; }

    public Guid? InitiatedByUserId { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? FailedAt { get; set; }

    public string? FailureReason { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("dispatch_run")]
    public virtual ICollection<DispatchCandidate> DispatchCandidates { get; set; } = new List<DispatchCandidate>();

    [ForeignKey("initiated_by_user_id")]
    [InverseProperty("dispatch_runs")]
    public virtual User? InitiatedByUser { get; set; }

    [ForeignKey("job_request_id")]
    [InverseProperty("dispatch_runs")]
    public virtual JobRequest JobRequest { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("dispatch_runs")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class DispatchRun
{
    public DispatchRunStatus status { get; set; }
}
// </auto-enum-partial>
