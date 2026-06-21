using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Identity.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VNotaryComplianceStatus
{
    public Guid? NotaryId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(200)]
    public string? PublicDisplayName { get; set; }

    public DateOnly? CommissionExpirationDate { get; set; }

    public DateOnly? BondExpirationDate { get; set; }

    public DateOnly? InsuranceExpirationDate { get; set; }
}

// <auto-enum-partial>
public partial class VNotaryComplianceStatus
{
    public BondStatus bond_status { get; set; }
    public CommissionStatus commission_status { get; set; }
    public InsuranceStatus insurance_status { get; set; }
    public NotaryStatus notary_status { get; set; }
}
// </auto-enum-partial>
