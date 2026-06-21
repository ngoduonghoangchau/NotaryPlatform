using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Billing;

/// <summary>
/// Customer credit balances and credit memos.
/// </summary>
[Table("credits", Schema = "billing")]
[Index("customer_id", Name = "ix_credits_customer_id")]
[Index("expires_on", Name = "ix_credits_expires_on")]
[Index("tenant_id", Name = "ix_credits_tenant_id")]
[Index("tenant_id", "credit_code", Name = "uq_credits_tenant_code", IsUnique = true)]
public partial class Credit
{
    [Key]
    public Guid CreditId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string CreditCode { get; set; } = null!;

    public Guid CustomerId { get; set; }

    public Guid? InvoiceId { get; set; }

    public Guid? OriginPaymentId { get; set; }

    public DateOnly CreditDate { get; set; }

    [StringLength(3)]
    public string CurrencyCode { get; set; } = null!;

    [Precision(18, 2)]
    public decimal OriginalAmount { get; set; }

    [Precision(18, 2)]
    public decimal AvailableAmount { get; set; }

    [Precision(18, 2)]
    public decimal UsedAmount { get; set; }

    [Precision(18, 2)]
    public decimal ExpiredAmount { get; set; }

    public DateOnly? ExpiresOn { get; set; }

    public string? Reason { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("creditcreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [InverseProperty("credit")]
    public virtual ICollection<CreditApplication> CreditApplications { get; set; } = new List<CreditApplication>();

    [ForeignKey("customer_id")]
    [InverseProperty("credits")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("invoice_id")]
    [InverseProperty("credits")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("origin_payment_id")]
    [InverseProperty("credits")]
    public virtual Payment? OriginPayment { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("credits")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("creditupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class Credit
{
    public CreditStatus credit_status { get; set; }
}
// </auto-enum-partial>
