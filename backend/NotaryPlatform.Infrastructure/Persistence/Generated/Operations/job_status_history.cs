using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Immutable status transition log for jobs.
/// </summary>
[Table("job_status_history", Schema = "operations")]
[Index("effective_at", Name = "ix_job_status_history_effective_at")]
[Index("job_id", Name = "ix_job_status_history_job_id")]
[Index("tenant_id", Name = "ix_job_status_history_tenant_id")]
public partial class JobStatusHistory
{
    [Key]
    public Guid JobStatusHistoryId { get; set; }

    public Guid TenantId { get; set; }

    public Guid JobId { get; set; }

    public string? Reason { get; set; }

    public string? SourceReference { get; set; }

    public DateTime EffectiveAt { get; set; }

    public Guid? ChangedByUserId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("changed_by_user_id")]
    [InverseProperty("job_status_histories")]
    public virtual User? ChangedByUser { get; set; }

    [ForeignKey("job_id")]
    [InverseProperty("job_status_histories")]
    public virtual Job Job { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("job_status_histories")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class JobStatusHistory
{
    public JobStatus new_status { get; set; }
    public JobStatus previous_status { get; set; }
}
// </auto-enum-partial>
