using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Flexible event timeline for the job detail screen.
/// </summary>
[Table("job_timeline_events", Schema = "operations")]
[Index("job_id", Name = "ix_job_timeline_events_job_id")]
[Index("occurred_at", Name = "ix_job_timeline_events_occurred_at")]
[Index("tenant_id", Name = "ix_job_timeline_events_tenant_id")]
public partial class JobTimelineEvent
{
    [Key]
    public Guid JobTimelineEventId { get; set; }

    public Guid TenantId { get; set; }

    public Guid JobId { get; set; }

    [StringLength(200)]
    public string? EventTitle { get; set; }

    public string? EventBody { get; set; }

    public Guid? RelatedAssignmentId { get; set; }

    public Guid? RelatedUserId { get; set; }

    public Guid? RelatedNotaryId { get; set; }

    public DateTime OccurredAt { get; set; }

    public Guid? OccurredByUserId { get; set; }

    public string? SourceReference { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("job_id")]
    [InverseProperty("job_timeline_events")]
    public virtual Job Job { get; set; } = null!;

    [ForeignKey("occurred_by_user_id")]
    [InverseProperty("job_timeline_eventoccurred_by_users")]
    public virtual User? OccurredByUser { get; set; }

    [ForeignKey("related_assignment_id")]
    [InverseProperty("job_timeline_events")]
    public virtual JobAssignment? RelatedAssignment { get; set; }

    [ForeignKey("related_notary_id")]
    [InverseProperty("job_timeline_events")]
    public virtual Notary? RelatedNotary { get; set; }

    [ForeignKey("related_user_id")]
    [InverseProperty("job_timeline_eventrelated_users")]
    public virtual User? RelatedUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("job_timeline_events")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class JobTimelineEvent
{
    public TimelineEventType event_type { get; set; }
    public JobStatus new_status { get; set; }
    public JobStatus old_status { get; set; }
}
// </auto-enum-partial>
