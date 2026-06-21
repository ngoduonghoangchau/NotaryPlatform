using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Billing;

/// <summary>
/// Refund transactions and statuses.
/// </summary>
[Table("refunds", Schema = "billing")]
[Index("customer_id", Name = "ix_refunds_customer_id")]
[Index("payment_id", Name = "ix_refunds_payment_id")]
[Index("tenant_id", Name = "ix_refunds_tenant_id")]
[Index("tenant_id", "refund_code", Name = "uq_refunds_tenant_code", IsUnique = true)]
public partial class Refund
{
    [Key]
    public Guid RefundId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string RefundCode { get; set; } = null!;

    public Guid PaymentId { get; set; }

    public Guid? InvoiceId { get; set; }

    public Guid CustomerId { get; set; }

    public DateOnly RefundDate { get; set; }

    [StringLength(3)]
    public string CurrencyCode { get; set; } = null!;

    [Precision(18, 2)]
    public decimal Amount { get; set; }

    public string Reason { get; set; } = null!;

    public string? Notes { get; set; }

    public string? ProviderRefundRef { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public DateTime? FailedAt { get; set; }

    public string? FailureReason { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("refundcreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("refunds")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("invoice_id")]
    [InverseProperty("refunds")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("payment_id")]
    [InverseProperty("refunds")]
    public virtual Payment Payment { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("refunds")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("refundupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class Refund
{
    public RefundStatus refund_status { get; set; }
}
// </auto-enum-partial>
