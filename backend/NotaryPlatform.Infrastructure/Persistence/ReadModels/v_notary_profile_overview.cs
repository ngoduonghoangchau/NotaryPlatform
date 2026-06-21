using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Core.Enums;
using NotaryPlatform.Domain.Features.Identity.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VNotaryProfileOverview
{
    public Guid? NotaryId { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? UserId { get; set; }

    [StringLength(50)]
    public string? InternalNotaryCode { get; set; }

    [StringLength(200)]
    public string? PublicDisplayName { get; set; }

    [StringLength(2)]
    public string? CommissioningStateCode { get; set; }

    [StringLength(100)]
    public string? CommissionNumber { get; set; }

    public DateOnly? CommissionIssueDate { get; set; }

    public DateOnly? CommissionExpirationDate { get; set; }

    [StringLength(50)]
    public string? EmploymentType { get; set; }

    public DateOnly? StartDate { get; set; }

    [Precision(5, 2)]
    public decimal? ErrorRate { get; set; }

    [Precision(3, 2)]
    public decimal? CustomerRating { get; set; }

    public int? TotalJobsCompleted { get; set; }

    [Column(TypeName = "citext")]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(200)]
    public string? DisplayName { get; set; }
}

// <auto-enum-partial>
public partial class VNotaryProfileOverview
{
    public NotaryStatus status { get; set; }
    public UserStatus user_status { get; set; }
}
// </auto-enum-partial>
