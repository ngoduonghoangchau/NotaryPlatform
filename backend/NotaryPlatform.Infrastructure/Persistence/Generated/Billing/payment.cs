using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Billing;

/// <summary>
/// Payment transactions and gateway references.
/// </summary>
[Table("payments", Schema = "billing")]
[Index("customer_id", Name = "ix_payments_customer_id")]
[Index("invoice_id", Name = "ix_payments_invoice_id")]
[Index("payment_date", Name = "ix_payments_payment_date")]
[Index("tenant_id", Name = "ix_payments_tenant_id")]
[Index("tenant_id", "payment_code", Name = "uq_payments_tenant_code", IsUnique = true)]
public partial class Payment
{
    [Key]
    public Guid PaymentId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string PaymentCode { get; set; } = null!;

    public Guid CustomerId { get; set; }

    public Guid? InvoiceId { get; set; }

    public Guid? PaymentMethodId { get; set; }

    public DateOnly PaymentDate { get; set; }

    public DateTime PaymentTime { get; set; }

    [StringLength(3)]
    public string CurrencyCode { get; set; } = null!;

    [Precision(18, 2)]
    public decimal Amount { get; set; }

    [Precision(18, 2)]
    public decimal FeeAmount { get; set; }

    [Precision(18, 2)]
    public decimal NetAmount { get; set; }

    [StringLength(200)]
    public string? ProviderName { get; set; }

    public string? ProviderTransactionRef { get; set; }

    public string? AuthorizationCode { get; set; }

    public string? SettlementBatchRef { get; set; }

    public DateTime? CapturedAt { get; set; }

    public DateTime? SettledAt { get; set; }

    public DateTime? FailedAt { get; set; }

    public DateTime? ReversedAt { get; set; }

    public string? FailureReason { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("payment")]
    public virtual ICollection<BillingAdjustment> BillingAdjustments { get; set; } = new List<BillingAdjustment>();

    [InverseProperty("payment")]
    public virtual ICollection<CommunicationThread> CommunicationThreads { get; set; } = new List<CommunicationThread>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("paymentcreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [InverseProperty("origin_payment")]
    public virtual ICollection<Credit> Credits { get; set; } = new List<Credit>();

    [ForeignKey("customer_id")]
    [InverseProperty("payments")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("invoice_id")]
    [InverseProperty("payments")]
    public virtual Invoice? Invoice { get; set; }

    [InverseProperty("payment")]
    public virtual ICollection<NotaryCommissionsPayable> NotaryCommissionsPayables { get; set; } = new List<NotaryCommissionsPayable>();

    [InverseProperty("payment")]
    public virtual ICollection<PaymentAllocation> PaymentAllocations { get; set; } = new List<PaymentAllocation>();

    [ForeignKey("payment_method_id")]
    [InverseProperty("payments")]
    public virtual PaymentMethod? PaymentMethod { get; set; }

    [InverseProperty("payment")]
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    [ForeignKey("tenant_id")]
    [InverseProperty("payments")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("paymentupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class Payment
{
    public PaymentStatus payment_status { get; set; }
}
// </auto-enum-partial>
