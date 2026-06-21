using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VRevenueShareSummary
{
    public Guid? TenantId { get; set; }

    public Guid? NotaryId { get; set; }

    [StringLength(200)]
    public string? NotaryName { get; set; }

    public long? ShareCount { get; set; }

    public decimal? TotalNetShare { get; set; }
}

// <auto-enum-partial>
public partial class VRevenueShareSummary
{
    public PayableStatus status { get; set; }
}
// </auto-enum-partial>
