using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Billing;

/// <summary>
/// Customer invoices and AR head record.
/// </summary>
[Table("invoices", Schema = "billing")]
[Index("contract_id", Name = "ix_invoices_contract_id")]
[Index("customer_id", Name = "ix_invoices_customer_id")]
[Index("due_date", Name = "ix_invoices_due_date")]
[Index("invoice_date", Name = "ix_invoices_invoice_date")]
[Index("tenant_id", Name = "ix_invoices_tenant_id")]
[Index("tenant_id", "invoice_code", Name = "uq_invoices_tenant_code", IsUnique = true)]
[Index("tenant_id", "invoice_number", Name = "uq_invoices_tenant_number", IsUnique = true)]
public partial class Invoice
{
    [Key]
    public Guid InvoiceId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string InvoiceCode { get; set; } = null!;

    [StringLength(100)]
    public string InvoiceNumber { get; set; } = null!;

    public Guid CustomerId { get; set; }

    public Guid? CustomerContactId { get; set; }

    public Guid? ContractId { get; set; }

    public Guid? SlaId { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public DateOnly InvoiceDate { get; set; }

    public DateOnly DueDate { get; set; }

    public DateTime? IssuedAt { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? VoidedAt { get; set; }

    public DateTime? WrittenOffAt { get; set; }

    [StringLength(3)]
    public string CurrencyCode { get; set; } = null!;

    [Precision(18, 6)]
    public decimal ExchangeRate { get; set; }

    [Precision(18, 2)]
    public decimal SubtotalAmount { get; set; }

    [Precision(18, 2)]
    public decimal DiscountAmount { get; set; }

    [Precision(18, 2)]
    public decimal TaxAmount { get; set; }

    [Precision(18, 2)]
    public decimal TotalAmount { get; set; }

    [Precision(18, 2)]
    public decimal PaidAmount { get; set; }

    [Precision(18, 2)]
    public decimal BalanceDue { get; set; }

    public int AgingDays { get; set; }

    [Column(TypeName = "citext")]
    public string? BillingEmail { get; set; }

    [StringLength(30)]
    public string? BillingPhone { get; set; }

    [StringLength(200)]
    public string? BillingAddressLine1 { get; set; }

    [StringLength(200)]
    public string? BillingAddressLine2 { get; set; }

    [StringLength(100)]
    public string? BillingCity { get; set; }

    [StringLength(2)]
    public string? BillingStateCode { get; set; }

    [StringLength(20)]
    public string? BillingPostalCode { get; set; }

    [StringLength(2)]
    public string? BillingCountryCode { get; set; }

    [StringLength(50)]
    public string? SendMethod { get; set; }

    public string? ExternalReference { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("invoice")]
    public virtual ICollection<BillingAdjustment> BillingAdjustments { get; set; } = new List<BillingAdjustment>();

    [ForeignKey("branch_id")]
    [InverseProperty("invoices")]
    public virtual Branch? Branch { get; set; }

    [InverseProperty("invoice")]
    public virtual ICollection<CommunicationReminder> CommunicationReminders { get; set; } = new List<CommunicationReminder>();

    [InverseProperty("invoice")]
    public virtual ICollection<CommunicationThread> CommunicationThreads { get; set; } = new List<CommunicationThread>();

    [ForeignKey("contract_id")]
    [InverseProperty("invoices")]
    public virtual Contract? Contract { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("invoicecreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [InverseProperty("invoice")]
    public virtual ICollection<CreditApplication> CreditApplications { get; set; } = new List<CreditApplication>();

    [InverseProperty("invoice")]
    public virtual ICollection<Credit> Credits { get; set; } = new List<Credit>();

    [ForeignKey("customer_id")]
    [InverseProperty("invoices")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("customer_contact_id")]
    [InverseProperty("invoices")]
    public virtual CustomerContact? CustomerContact { get; set; }

    [InverseProperty("invoice")]
    public virtual ICollection<InternalNote> InternalNotes { get; set; } = new List<InternalNote>();

    [InverseProperty("invoice")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    [InverseProperty("invoice")]
    public virtual ICollection<NotaryCommissionsPayable> NotaryCommissionsPayables { get; set; } = new List<NotaryCommissionsPayable>();

    [InverseProperty("invoice")]
    public virtual ICollection<PaymentAllocation> PaymentAllocations { get; set; } = new List<PaymentAllocation>();

    [InverseProperty("invoice")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("invoice")]
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    [ForeignKey("region_id")]
    [InverseProperty("invoices")]
    public virtual Region? Region { get; set; }

    [InverseProperty("invoice")]
    public virtual ICollection<RevenueShare> RevenueShares { get; set; } = new List<RevenueShare>();

    [ForeignKey("sla_id")]
    [InverseProperty("invoices")]
    public virtual SlaAgreement? Sla { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("invoices")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("invoiceupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class Invoice
{
    public AgingBucket aging_bucket { get; set; }
    public InvoiceStatus invoice_status { get; set; }
}
// </auto-enum-partial>
