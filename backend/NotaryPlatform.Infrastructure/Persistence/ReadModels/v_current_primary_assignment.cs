using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VCurrentPrimaryAssignment
{
    public Guid? JobAssignmentId { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? JobId { get; set; }

    public Guid? NotaryId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    [Column(TypeName = "jsonb")]
    public string? ComplianceSnapshot { get; set; }

    [Column(TypeName = "jsonb")]
    public string? PerformanceSnapshot { get; set; }
}

// <auto-enum-partial>
public partial class VCurrentPrimaryAssignment
{
    public AssignmentRole assignment_role { get; set; }
    public AssignmentStatus status { get; set; }
}
// </auto-enum-partial>
