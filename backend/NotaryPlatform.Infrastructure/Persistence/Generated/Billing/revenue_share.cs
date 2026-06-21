using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Billing;

/// <summary>
/// Revenue share calculations for jobs, acts, invoices, and notaries.
/// </summary>
[Table("revenue_shares", Schema = "billing")]
[Index("invoice_id", Name = "ix_revenue_shares_invoice_id")]
[Index("notary_id", Name = "ix_revenue_shares_notary_id")]
[Index("tenant_id", Name = "ix_revenue_shares_tenant_id")]
[Index("tenant_id", "revenue_share_code", Name = "uq_revenue_shares_tenant_code", IsUnique = true)]
public partial class RevenueShare
{
    [Key]
    public Guid RevenueShareId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string RevenueShareCode { get; set; } = null!;

    public Guid? CustomerId { get; set; }

    public Guid? ContractId { get; set; }

    public Guid? InvoiceId { get; set; }

    public Guid? JobId { get; set; }

    public Guid? NotarialActId { get; set; }

    public Guid? JournalEntryId { get; set; }

    public Guid? NotaryId { get; set; }

    [Precision(18, 2)]
    public decimal ShareBasisAmount { get; set; }

    [Precision(5, 2)]
    public decimal? ShareRatePercent { get; set; }

    [Precision(18, 2)]
    public decimal? FixedShareAmount { get; set; }

    [Precision(18, 2)]
    public decimal GrossShareAmount { get; set; }

    [Precision(18, 2)]
    public decimal DeductionsAmount { get; set; }

    [Precision(18, 2)]
    public decimal NetShareAmount { get; set; }

    public DateTime? AccruedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? ReversedAt { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("contract_id")]
    [InverseProperty("revenue_shares")]
    public virtual Contract? Contract { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("revenue_sharecreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("revenue_shares")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("invoice_id")]
    [InverseProperty("revenue_shares")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("job_id")]
    [InverseProperty("revenue_shares")]
    public virtual Job? Job { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("revenue_shares")]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey("notarial_act_id")]
    [InverseProperty("revenue_shares")]
    public virtual NotarialAct? NotarialAct { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("revenue_shares")]
    public virtual Notary? Notary { get; set; }

    [InverseProperty("revenue_share")]
    public virtual ICollection<NotaryCommissionsPayable> NotaryCommissionsPayables { get; set; } = new List<NotaryCommissionsPayable>();

    [ForeignKey("tenant_id")]
    [InverseProperty("revenue_shares")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("revenue_shareupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class RevenueShare
{
    public RevenueShareType revenue_share_type { get; set; }
    public PayableStatus status { get; set; }
}
// </auto-enum-partial>
