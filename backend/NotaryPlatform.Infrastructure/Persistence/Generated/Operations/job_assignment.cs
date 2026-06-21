using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// History of notary assignments for each job.
/// </summary>
[Table("job_assignments", Schema = "operations")]
[Index("is_primary", Name = "ix_job_assignments_is_primary")]
[Index("job_id", Name = "ix_job_assignments_job_id")]
[Index("notary_id", Name = "ix_job_assignments_notary_id")]
[Index("tenant_id", Name = "ix_job_assignments_tenant_id")]
public partial class JobAssignment
{
    [Key]
    public Guid JobAssignmentId { get; set; }

    public Guid TenantId { get; set; }

    public Guid JobId { get; set; }

    public Guid NotaryId { get; set; }

    public bool IsPrimary { get; set; }

    public Guid? AssignedByUserId { get; set; }

    public Guid? ReleasedByUserId { get; set; }

    public DateTime ProposedAt { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public DateTime? DeclinedAt { get; set; }

    public DateTime? ReleasedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    [Precision(10, 2)]
    public decimal? DistanceKm { get; set; }

    public int? TravelMinutes { get; set; }

    [Column(TypeName = "jsonb")]
    public string ComplianceSnapshot { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string PerformanceSnapshot { get; set; } = null!;

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("assigned_by_user_id")]
    [InverseProperty("job_assignmentassigned_by_users")]
    public virtual User? AssignedByUser { get; set; }

    [ForeignKey("job_id")]
    [InverseProperty("job_assignment")]
    public virtual Job Job { get; set; } = null!;

    [InverseProperty("related_assignment")]
    public virtual ICollection<JobTimelineEvent> JobTimelineEvents { get; set; } = new List<JobTimelineEvent>();

    [ForeignKey("notary_id")]
    [InverseProperty("job_assignments")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("released_by_user_id")]
    [InverseProperty("job_assignmentreleased_by_users")]
    public virtual User? ReleasedByUser { get; set; }

    [InverseProperty("job_assignment")]
    public virtual ICollection<ScheduleBlock> ScheduleBlocks { get; set; } = new List<ScheduleBlock>();

    [ForeignKey("tenant_id")]
    [InverseProperty("job_assignments")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class JobAssignment
{
    public AssignmentRole assignment_role { get; set; }
    public AssignmentStatus status { get; set; }
}
// </auto-enum-partial>
