using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VInvoiceOverview
{
    public Guid? InvoiceId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(50)]
    public string? InvoiceCode { get; set; }

    [StringLength(100)]
    public string? InvoiceNumber { get; set; }

    public Guid? CustomerId { get; set; }

    [StringLength(200)]
    public string? CustomerName { get; set; }

    public Guid? ContractId { get; set; }

    public Guid? SlaId { get; set; }

    public DateOnly? InvoiceDate { get; set; }

    public DateOnly? DueDate { get; set; }

    [StringLength(3)]
    public string? CurrencyCode { get; set; }

    [Precision(18, 2)]
    public decimal? SubtotalAmount { get; set; }

    [Precision(18, 2)]
    public decimal? DiscountAmount { get; set; }

    [Precision(18, 2)]
    public decimal? TaxAmount { get; set; }

    [Precision(18, 2)]
    public decimal? TotalAmount { get; set; }

    [Precision(18, 2)]
    public decimal? PaidAmount { get; set; }

    [Precision(18, 2)]
    public decimal? BalanceDue { get; set; }

    public int? AgingDays { get; set; }

    public DateTime? IssuedAt { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? VoidedAt { get; set; }
}

// <auto-enum-partial>
public partial class VInvoiceOverview
{
    public AgingBucket aging_bucket { get; set; }
    public InvoiceStatus invoice_status { get; set; }
}
// </auto-enum-partial>
