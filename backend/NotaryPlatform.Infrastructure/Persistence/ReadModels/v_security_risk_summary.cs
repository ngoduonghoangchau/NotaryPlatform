using Microsoft.EntityFrameworkCore;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VSecurityRiskSummary
{
    public Guid? TenantId { get; set; }

    public long? ActiveSeals { get; set; }

    public long? RiskSeals { get; set; }

    public long? SealsExpiringSoon { get; set; }
}
