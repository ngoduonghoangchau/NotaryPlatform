using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Master scheduling board entries and calendar blocks.
/// </summary>
[Table("schedule_blocks", Schema = "operations")]
[Index("end_at", Name = "ix_schedule_blocks_end_at")]
[Index("job_assignment_id", Name = "ix_schedule_blocks_job_assignment_id")]
[Index("job_id", Name = "ix_schedule_blocks_job_id")]
[Index("notary_id", Name = "ix_schedule_blocks_notary_id")]
[Index("start_at", Name = "ix_schedule_blocks_start_at")]
[Index("tenant_id", Name = "ix_schedule_blocks_tenant_id")]
[Index("tenant_id", "block_code", Name = "uq_schedule_blocks_tenant_code", IsUnique = true)]
public partial class ScheduleBlock
{
    [Key]
    public Guid ScheduleBlockId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string BlockCode { get; set; } = null!;

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    [StringLength(64)]
    public string Timezone { get; set; } = null!;

    public Guid? JobId { get; set; }

    public Guid? JobAssignmentId { get; set; }

    public Guid? NotaryId { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    [StringLength(200)]
    public string? LocationName { get; set; }

    [StringLength(200)]
    public string? AddressLine1 { get; set; }

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(2)]
    public string? CountryCode { get; set; }

    [Precision(10, 7)]
    public decimal? Latitude { get; set; }

    [Precision(10, 7)]
    public decimal? Longitude { get; set; }

    public bool IsAllDay { get; set; }

    public bool IsConflict { get; set; }

    public string? ConflictReason { get; set; }

    public string? SourceReference { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("schedule_blocks")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("schedule_blockcreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("job_id")]
    [InverseProperty("schedule_blocks")]
    public virtual Job? Job { get; set; }

    [ForeignKey("job_assignment_id")]
    [InverseProperty("schedule_blocks")]
    public virtual JobAssignment? JobAssignment { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("schedule_blocks")]
    public virtual Notary? Notary { get; set; }

    [ForeignKey("region_id")]
    [InverseProperty("schedule_blocks")]
    public virtual Region? Region { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("schedule_blocks")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("schedule_blockupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class ScheduleBlock
{
    public ScheduleBlockType block_type { get; set; }
}
// </auto-enum-partial>
