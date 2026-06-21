using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Billing;

/// <summary>
/// Manual billing adjustments and corrections.
/// </summary>
[Table("billing_adjustments", Schema = "billing")]
[Index("customer_id", Name = "ix_billing_adjustments_customer_id")]
[Index("invoice_id", Name = "ix_billing_adjustments_invoice_id")]
[Index("tenant_id", Name = "ix_billing_adjustments_tenant_id")]
[Index("tenant_id", "adjustment_code", Name = "uq_billing_adjustments_tenant_code", IsUnique = true)]
public partial class BillingAdjustment
{
    [Key]
    public Guid BillingAdjustmentId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string AdjustmentCode { get; set; } = null!;

    public Guid? InvoiceId { get; set; }

    public Guid? PaymentId { get; set; }

    public Guid CustomerId { get; set; }

    public DateOnly AdjustmentDate { get; set; }

    [StringLength(3)]
    public string CurrencyCode { get; set; } = null!;

    [Precision(18, 2)]
    public decimal Amount { get; set; }

    public string Reason { get; set; } = null!;

    public string? Notes { get; set; }

    public string? SourceReference { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("billing_adjustmentcreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("billing_adjustments")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("invoice_id")]
    [InverseProperty("billing_adjustments")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("payment_id")]
    [InverseProperty("billing_adjustments")]
    public virtual Payment? Payment { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("billing_adjustments")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("billing_adjustmentupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class BillingAdjustment
{
    public AdjustmentStatus adjustment_status { get; set; }
    public InvoiceItemType adjustment_type { get; set; }
}
// </auto-enum-partial>
