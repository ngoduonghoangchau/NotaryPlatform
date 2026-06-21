using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Commercial pricing plan linked to a customer.
/// </summary>
[Table("pricing_plans", Schema = "crm")]
[Index("customer_id", Name = "ix_pricing_plans_customer_id")]
[Index("tenant_id", Name = "ix_pricing_plans_tenant_id")]
[Index("customer_id", "plan_code", Name = "uq_pricing_plans_customer_code", IsUnique = true)]
public partial class PricingPlan
{
    [Key]
    public Guid PricingPlanId { get; set; }

    public Guid TenantId { get; set; }

    public Guid CustomerId { get; set; }

    [StringLength(50)]
    public string PlanCode { get; set; } = null!;

    [StringLength(200)]
    public string PlanName { get; set; } = null!;

    [StringLength(3)]
    public string CurrencyCode { get; set; } = null!;

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    [Precision(18, 2)]
    public decimal? BaseRate { get; set; }

    [Precision(18, 2)]
    public decimal? MinimumMonthlyFee { get; set; }

    public int? VolumeThreshold { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("pricing_plan")]
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("pricing_plans")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("customer_id")]
    [InverseProperty("pricing_plans")]
    public virtual Customer Customer { get; set; } = null!;

    [InverseProperty("pricing_plan")]
    public virtual ICollection<PricingRule> PricingRules { get; set; } = new List<PricingRule>();

    [ForeignKey("tenant_id")]
    [InverseProperty("pricing_plans")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class PricingPlan
{
    public PricingModel pricing_model { get; set; }
    public ContractStatus status { get; set; }
}
// </auto-enum-partial>
