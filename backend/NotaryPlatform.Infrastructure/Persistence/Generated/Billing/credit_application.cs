using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Billing;

/// <summary>
/// Application of credits to invoices.
/// </summary>
[Table("credit_applications", Schema = "billing")]
[Index("credit_id", Name = "ix_credit_applications_credit_id")]
[Index("invoice_id", Name = "ix_credit_applications_invoice_id")]
[Index("tenant_id", Name = "ix_credit_applications_tenant_id")]
[Index("credit_id", "invoice_id", Name = "uq_credit_applications_credit_invoice", IsUnique = true)]
[Index("tenant_id", "application_code", Name = "uq_credit_applications_tenant_code", IsUnique = true)]
public partial class CreditApplication
{
    [Key]
    public Guid CreditApplicationId { get; set; }

    public Guid TenantId { get; set; }

    public Guid CreditId { get; set; }

    public Guid InvoiceId { get; set; }

    [StringLength(50)]
    public string ApplicationCode { get; set; } = null!;

    [Precision(18, 2)]
    public decimal AppliedAmount { get; set; }

    public DateTime AppliedAt { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("credit_id")]
    [InverseProperty("credit_applications")]
    public virtual Credit Credit { get; set; } = null!;

    [ForeignKey("invoice_id")]
    [InverseProperty("credit_applications")]
    public virtual Invoice Invoice { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("credit_applications")]
    public virtual Tenant Tenant { get; set; } = null!;
}
