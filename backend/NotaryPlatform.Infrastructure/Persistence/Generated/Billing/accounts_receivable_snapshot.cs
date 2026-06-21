using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Billing;

/// <summary>
/// AR aging snapshots for dashboards and reporting.
/// </summary>
[Table("accounts_receivable_snapshots", Schema = "billing")]
[Index("customer_id", Name = "ix_ar_snapshots_customer_id")]
[Index("snapshot_date", Name = "ix_ar_snapshots_snapshot_date")]
[Index("tenant_id", Name = "ix_ar_snapshots_tenant_id")]
[Index("tenant_id", "snapshot_date", "customer_id", "branch_id", "region_id", Name = "uq_ar_snapshots_unique_scope", IsUnique = true)]
public partial class AccountsReceivableSnapshot
{
    [Key]
    public Guid ArSnapshotId { get; set; }

    public Guid TenantId { get; set; }

    public DateOnly SnapshotDate { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    [Precision(18, 2)]
    public decimal CurrentBalance { get; set; }

    [Precision(18, 2)]
    public decimal Days130Balance { get; set; }

    [Precision(18, 2)]
    public decimal Days3160Balance { get; set; }

    [Precision(18, 2)]
    public decimal Days6190Balance { get; set; }

    [Precision(18, 2)]
    public decimal Days90PlusBalance { get; set; }

    public int InvoiceCount { get; set; }

    public int OverdueInvoiceCount { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metrics { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("accounts_receivable_snapshots")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("accounts_receivable_snapshots")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("region_id")]
    [InverseProperty("accounts_receivable_snapshots")]
    public virtual Region? Region { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("accounts_receivable_snapshots")]
    public virtual Tenant Tenant { get; set; } = null!;
}
