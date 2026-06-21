using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VCurrentPolicyVersion
{
    public Guid? PolicyVersionId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(50)]
    public string? PolicyCode { get; set; }

    [StringLength(200)]
    public string? PolicyName { get; set; }

    public int? VersionNo { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public DateTime? PublishedAt { get; set; }
}

// <auto-enum-partial>
public partial class VCurrentPolicyVersion
{
    public PolicyStatus policy_status { get; set; }
    public RuleScope scope { get; set; }
}
// </auto-enum-partial>
