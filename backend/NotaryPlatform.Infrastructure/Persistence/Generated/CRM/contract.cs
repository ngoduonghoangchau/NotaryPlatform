using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Commercial contracts with B2B customers.
/// </summary>
[Table("contracts", Schema = "crm")]
[Index("customer_id", Name = "ix_contracts_customer_id")]
[Index("effective_date", Name = "ix_contracts_effective_date")]
[Index("expiration_date", Name = "ix_contracts_expiration_date")]
[Index("tenant_id", Name = "ix_contracts_tenant_id")]
[Index("customer_id", "contract_number", Name = "uq_contracts_customer_number", IsUnique = true)]
public partial class Contract
{
    [Key]
    public Guid ContractId { get; set; }

    public Guid TenantId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? PricingPlanId { get; set; }

    [StringLength(100)]
    public string ContractNumber { get; set; } = null!;

    [StringLength(200)]
    public string ContractName { get; set; } = null!;

    public DateOnly EffectiveDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public DateOnly? SignedDate { get; set; }

    public bool AutoRenew { get; set; }

    public int RenewalNoticeDays { get; set; }

    public string? SlaSummary { get; set; }

    public string? CommercialTerms { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("contract")]
    public virtual ICollection<ContractDocument> ContractDocuments { get; set; } = new List<ContractDocument>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("contractcreated_by_users")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("customer_id")]
    [InverseProperty("contracts")]
    public virtual Customer Customer { get; set; } = null!;

    [InverseProperty("contract")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [ForeignKey("pricing_plan_id")]
    [InverseProperty("contracts")]
    public virtual PricingPlan? PricingPlan { get; set; }

    [InverseProperty("contract")]
    public virtual ICollection<RevenueShare> RevenueShares { get; set; } = new List<RevenueShare>();

    [InverseProperty("contract")]
    public virtual ICollection<SlaAgreement> SlaAgreements { get; set; } = new List<SlaAgreement>();

    [ForeignKey("tenant_id")]
    [InverseProperty("contracts")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("contractupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class Contract
{
    public ContractStatus status { get; set; }
}
// </auto-enum-partial>
