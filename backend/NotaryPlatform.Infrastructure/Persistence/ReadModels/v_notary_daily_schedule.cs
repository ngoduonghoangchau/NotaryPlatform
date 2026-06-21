using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VNotaryDailySchedule
{
    public Guid? ScheduleBlockId { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? NotaryId { get; set; }

    public Guid? JobId { get; set; }

    public Guid? JobAssignmentId { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    [StringLength(64)]
    public string? Timezone { get; set; }

    [StringLength(200)]
    public string? LocationName { get; set; }

    public bool? IsAllDay { get; set; }

    public bool? IsConflict { get; set; }
}

// <auto-enum-partial>
public partial class VNotaryDailySchedule
{
    public ScheduleBlockType block_type { get; set; }
}
// </auto-enum-partial>
