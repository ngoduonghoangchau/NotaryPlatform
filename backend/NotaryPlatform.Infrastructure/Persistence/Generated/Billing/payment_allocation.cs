using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Billing;

/// <summary>
/// Allocations of payments to invoices.
/// </summary>
[Table("payment_allocations", Schema = "billing")]
[Index("invoice_id", Name = "ix_payment_allocations_invoice_id")]
[Index("payment_id", Name = "ix_payment_allocations_payment_id")]
[Index("tenant_id", Name = "ix_payment_allocations_tenant_id")]
[Index("payment_id", "invoice_id", Name = "uq_payment_allocations_payment_invoice", IsUnique = true)]
[Index("tenant_id", "allocation_code", Name = "uq_payment_allocations_tenant_code", IsUnique = true)]
public partial class PaymentAllocation
{
    [Key]
    public Guid PaymentAllocationId { get; set; }

    public Guid TenantId { get; set; }

    public Guid PaymentId { get; set; }

    public Guid InvoiceId { get; set; }

    [StringLength(50)]
    public string AllocationCode { get; set; } = null!;

    [Precision(18, 2)]
    public decimal AllocatedAmount { get; set; }

    public DateTime AppliedAt { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("invoice_id")]
    [InverseProperty("payment_allocations")]
    public virtual Invoice Invoice { get; set; } = null!;

    [ForeignKey("payment_id")]
    [InverseProperty("payment_allocations")]
    public virtual Payment Payment { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("payment_allocations")]
    public virtual Tenant Tenant { get; set; } = null!;
}
