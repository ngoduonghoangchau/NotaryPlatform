using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VActOverview
{
    public Guid? ActId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(50)]
    public string? ActCode { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public bool? OathRequired { get; set; }

    public bool? ThumbprintRequired { get; set; }

    public bool? IdentityVerificationRequired { get; set; }

    public bool? PersonalAppearanceConfirmed { get; set; }

    public bool? OathAdministered { get; set; }

    public DateTime? ActStartedAt { get; set; }

    public DateTime? ActCompletedAt { get; set; }

    public DateTime? ActLockedAt { get; set; }

    public DateTime? ActVoidedAt { get; set; }

    [StringLength(200)]
    public string? CustomerName { get; set; }

    [StringLength(200)]
    public string? NotaryName { get; set; }

    [StringLength(50)]
    public string? JobCode { get; set; }
}

// <auto-enum-partial>
public partial class VActOverview
{
    public NotarialActType act_type { get; set; }
    public AppearanceType appearance_type { get; set; }
    public NotarialActStatus status { get; set; }
}
// </auto-enum-partial>
