using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Billing;

/// <summary>
/// Invoice line items linked to jobs, acts, and journal entries.
/// </summary>
[Table("invoice_items", Schema = "billing")]
[Index("invoice_id", Name = "ix_invoice_items_invoice_id")]
[Index("job_id", Name = "ix_invoice_items_job_id")]
[Index("notarial_act_id", Name = "ix_invoice_items_notarial_act_id")]
[Index("tenant_id", Name = "ix_invoice_items_tenant_id")]
[Index("invoice_id", "line_no", Name = "uq_invoice_items_invoice_line", IsUnique = true)]
public partial class InvoiceItem
{
    [Key]
    public Guid InvoiceItemId { get; set; }

    public Guid TenantId { get; set; }

    public Guid InvoiceId { get; set; }

    public int LineNo { get; set; }

    public Guid? ServiceTypeId { get; set; }

    public Guid? JobId { get; set; }

    public Guid? NotarialActId { get; set; }

    public Guid? JournalEntryId { get; set; }

    [StringLength(50)]
    public string? ItemCode { get; set; }

    [StringLength(300)]
    public string Description { get; set; } = null!;

    [Precision(18, 4)]
    public decimal Quantity { get; set; }

    [Precision(18, 2)]
    public decimal UnitPrice { get; set; }

    [Precision(18, 2)]
    public decimal DiscountAmount { get; set; }

    [Precision(18, 2)]
    public decimal TaxAmount { get; set; }

    [Precision(18, 2)]
    public decimal LineTotal { get; set; }

    [StringLength(50)]
    public string? UnitOfMeasure { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("invoice_id")]
    [InverseProperty("invoice_items")]
    public virtual Invoice Invoice { get; set; } = null!;

    [ForeignKey("job_id")]
    [InverseProperty("invoice_items")]
    public virtual Job? Job { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("invoice_items")]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey("notarial_act_id")]
    [InverseProperty("invoice_items")]
    public virtual NotarialAct? NotarialAct { get; set; }

    [ForeignKey("service_type_id")]
    [InverseProperty("invoice_items")]
    public virtual ServiceType? ServiceType { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("invoice_items")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class InvoiceItem
{
    public InvoiceItemType item_type { get; set; }
}
// </auto-enum-partial>
