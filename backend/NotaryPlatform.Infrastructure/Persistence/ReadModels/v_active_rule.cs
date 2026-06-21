using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VActiveRule
{
    public Guid? ComplianceRuleId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(50)]
    public string? RuleCode { get; set; }

    [StringLength(200)]
    public string? RuleName { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public bool? BlockOnFailure { get; set; }

    public bool? RequiresManualReview { get; set; }

    public bool? IsMandatory { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}

// <auto-enum-partial>
public partial class VActiveRule
{
    public RuleStatus rule_status { get; set; }
    public RuleScope scope { get; set; }
    public RuleSeverity severity { get; set; }
}
// </auto-enum-partial>
