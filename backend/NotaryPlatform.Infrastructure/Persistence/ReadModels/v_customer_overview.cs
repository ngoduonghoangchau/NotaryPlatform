using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VCustomerOverview
{
    public Guid? CustomerId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(50)]
    public string? CustomerCode { get; set; }

    [StringLength(200)]
    public string? DisplayName { get; set; }

    [StringLength(250)]
    public string? LegalName { get; set; }

    [StringLength(120)]
    public string? Industry { get; set; }

    [Column(TypeName = "citext")]
    public string? PrimaryEmail { get; set; }

    [StringLength(30)]
    public string? PrimaryPhone { get; set; }

    [Column(TypeName = "citext")]
    public string? BillingEmail { get; set; }

    [StringLength(30)]
    public string? BillingPhone { get; set; }

    public int? TotalJobs { get; set; }

    [Precision(18, 2)]
    public decimal? TotalRevenue { get; set; }

    public int? AvgTurnaroundMinutes { get; set; }

    public Guid? AccountManagerUserId { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

// <auto-enum-partial>
public partial class VCustomerOverview
{
    public CustomerType customer_type { get; set; }
    public CustomerStatus status { get; set; }
}
// </auto-enum-partial>
