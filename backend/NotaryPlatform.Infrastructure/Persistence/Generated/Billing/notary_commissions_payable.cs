using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Billing;

/// <summary>
/// Commission payable records for notaries.
/// </summary>
[Table("notary_commissions_payable", Schema = "billing")]
[Index("due_date", Name = "ix_notary_commissions_payable_due_date")]
[Index("notary_id", Name = "ix_notary_commissions_payable_notary_id")]
[Index("tenant_id", Name = "ix_notary_commissions_payable_tenant_id")]
[Index("tenant_id", "payable_code", Name = "uq_payables_tenant_code", IsUnique = true)]
public partial class NotaryCommissionsPayable
{
    [Key]
    public Guid PayableId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string PayableCode { get; set; } = null!;

    public Guid NotaryId { get; set; }

    public Guid? RevenueShareId { get; set; }

    public Guid? InvoiceId { get; set; }

    public Guid? PaymentId { get; set; }

    public DateOnly AccrualDate { get; set; }

    public DateOnly? DueDate { get; set; }

    [StringLength(3)]
    public string CurrencyCode { get; set; } = null!;

    [Precision(18, 2)]
    public decimal GrossAmount { get; set; }

    [Precision(18, 2)]
    public decimal DeductionsAmount { get; set; }

    [Precision(18, 2)]
    public decimal NetAmount { get; set; }

    [Precision(18, 2)]
    public decimal PaidAmount { get; set; }

    [Precision(18, 2)]
    public decimal BalanceDue { get; set; }

    [StringLength(100)]
    public string? PaymentMethodText { get; set; }

    public string? PayoutReference { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? ApprovedByUserId { get; set; }

    public Guid? PaidByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? ReversedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("approved_by_user_id")]
    [InverseProperty("notary_commissions_payableapproved_by_users")]
    public virtual User? ApprovedByUser { get; set; }

    [ForeignKey("invoice_id")]
    [InverseProperty("notary_commissions_payables")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("notary_commissions_payables")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("paid_by_user_id")]
    [InverseProperty("notary_commissions_payablepaid_by_users")]
    public virtual User? PaidByUser { get; set; }

    [ForeignKey("payment_id")]
    [InverseProperty("notary_commissions_payables")]
    public virtual Payment? Payment { get; set; }

    [ForeignKey("revenue_share_id")]
    [InverseProperty("notary_commissions_payables")]
    public virtual RevenueShare? RevenueShare { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("notary_commissions_payables")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class NotaryCommissionsPayable
{
    public PayableStatus payable_status { get; set; }
}
// </auto-enum-partial>
