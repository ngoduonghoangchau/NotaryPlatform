using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VJobBoard
{
    public Guid? JobId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(50)]
    public string? JobCode { get; set; }

    [StringLength(50)]
    public string? ServiceCode { get; set; }

    [StringLength(200)]
    public string? ServiceName { get; set; }

    public Guid? CustomerId { get; set; }

    [StringLength(200)]
    public string? CustomerName { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public DateTime? RequestedStartAt { get; set; }

    public DateTime? RequestedEndAt { get; set; }

    public DateTime? ScheduledStartAt { get; set; }

    public DateTime? ScheduledEndAt { get; set; }

    public DateTime? SlaDueAt { get; set; }

    public bool? RushFlag { get; set; }

    public DateTime? LockedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? CancelledAt { get; set; }
}

// <auto-enum-partial>
public partial class VJobBoard
{
    public JobPriority priority { get; set; }
    public ServiceMode service_mode { get; set; }
    public JobStatus status { get; set; }
}
// </auto-enum-partial>
