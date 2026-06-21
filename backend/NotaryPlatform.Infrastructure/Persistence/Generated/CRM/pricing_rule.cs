using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Detailed pricing rules and tiers.
/// </summary>
[Table("pricing_rules", Schema = "crm")]
[Index("pricing_plan_id", Name = "ix_pricing_rules_pricing_plan_id")]
[Index("service_type", Name = "ix_pricing_rules_service_type")]
[Index("tenant_id", Name = "ix_pricing_rules_tenant_id")]
[Index("pricing_plan_id", "rule_code", Name = "uq_pricing_rules_plan_code", IsUnique = true)]
public partial class PricingRule
{
    [Key]
    public Guid PricingRuleId { get; set; }

    public Guid TenantId { get; set; }

    public Guid PricingPlanId { get; set; }

    [StringLength(50)]
    public string RuleCode { get; set; } = null!;

    [StringLength(200)]
    public string RuleName { get; set; } = null!;

    [StringLength(100)]
    public string? ServiceType { get; set; }

    public int? MinQuantity { get; set; }

    public int? MaxQuantity { get; set; }

    [Precision(18, 2)]
    public decimal? FlatFee { get; set; }

    [Precision(18, 2)]
    public decimal? UnitPrice { get; set; }

    [Precision(5, 2)]
    public decimal? DiscountPercent { get; set; }

    [Precision(18, 2)]
    public decimal? SurchargeAmount { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("pricing_plan_id")]
    [InverseProperty("pricing_rules")]
    public virtual PricingPlan PricingPlan { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("pricing_rules")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class PricingRule
{
    public PricingRuleType rule_type { get; set; }
}
// </auto-enum-partial>
