using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VActiveSeal
{
    public Guid? SealId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(50)]
    public string? SealCode { get; set; }

    [StringLength(200)]
    public string? SealName { get; set; }

    public Guid? NotaryId { get; set; }

    [StringLength(200)]
    public string? NotaryName { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    [StringLength(100)]
    public string? CommissionNumber { get; set; }

    [StringLength(100)]
    public string? SealNumber { get; set; }

    public DateOnly? EffectiveFrom { get; set; }

    public DateOnly? ExpiresOn { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public int? UsageCount { get; set; }
}

// <auto-enum-partial>
public partial class VActiveSeal
{
    public SealType seal_type { get; set; }
    public SealStatus status { get; set; }
}
// </auto-enum-partial>
