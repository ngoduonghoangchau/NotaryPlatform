using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VAgingSummary
{
    public Guid? TenantId { get; set; }

    public Guid? CustomerId { get; set; }

    public long? InvoiceCount { get; set; }

    public decimal? TotalBalanceDue { get; set; }
}

// <auto-enum-partial>
public partial class VAgingSummary
{
    public AgingBucket aging_bucket { get; set; }
}
// </auto-enum-partial>
