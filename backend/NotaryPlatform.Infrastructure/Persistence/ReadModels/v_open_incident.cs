using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VOpenIncident
{
    public Guid? IncidentId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(50)]
    public string? IncidentCode { get; set; }

    [StringLength(250)]
    public string? Title { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public DateTime? DetectedAt { get; set; }

    public DateTime? ReportedAt { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public bool? LegalHoldApplied { get; set; }
}

// <auto-enum-partial>
public partial class VOpenIncident
{
    public IncidentStatus incident_status { get; set; }
    public IncidentType incident_type { get; set; }
    public IncidentSeverity severity { get; set; }
}
// </auto-enum-partial>
