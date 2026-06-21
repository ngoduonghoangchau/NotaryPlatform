using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;

/// <summary>
/// Scheduled/executed retention jobs.
/// </summary>
[Table("retention_jobs", Schema = "compliance")]
[Index("entity_type", "entity_id", Name = "ix_retention_jobs_entity")]
[Index("retention_policy_id", Name = "ix_retention_jobs_policy_id")]
[Index("scheduled_at", Name = "ix_retention_jobs_scheduled_at")]
[Index("tenant_id", Name = "ix_retention_jobs_tenant_id")]
[Index("tenant_id", "retention_job_code", Name = "uq_retention_jobs_tenant_code", IsUnique = true)]
public partial class RetentionJob
{
    [Key]
    public Guid RetentionJobId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string RetentionJobCode { get; set; } = null!;

    public Guid RetentionPolicyId { get; set; }

    [StringLength(100)]
    public string EntityType { get; set; } = null!;

    public Guid EntityId { get; set; }

    public DateTime ScheduledAt { get; set; }

    public DateTime? ExecutedAt { get; set; }

    [StringLength(50)]
    public string JobStatus { get; set; } = null!;

    public string? Outcome { get; set; }

    public bool LegalHoldBlocked { get; set; }

    public string? BlockedReason { get; set; }

    public Guid? RequestedByUserId { get; set; }

    public Guid? ExecutedByUserId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("executed_by_user_id")]
    [InverseProperty("retention_jobexecuted_by_users")]
    public virtual User? ExecutedByUser { get; set; }

    [ForeignKey("requested_by_user_id")]
    [InverseProperty("retention_jobrequested_by_users")]
    public virtual User? RequestedByUser { get; set; }

    [ForeignKey("retention_policy_id")]
    [InverseProperty("retention_jobs")]
    public virtual RetentionPolicy RetentionPolicy { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("retention_jobs")]
    public virtual Tenant Tenant { get; set; } = null!;
}
