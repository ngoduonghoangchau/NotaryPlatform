using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;

/// <summary>
/// Corrective actions for incidents.
/// </summary>
[Table("incident_actions", Schema = "compliance")]
[Index("due_at", Name = "ix_incident_actions_due_at")]
[Index("incident_id", Name = "ix_incident_actions_incident_id")]
[Index("tenant_id", Name = "ix_incident_actions_tenant_id")]
[Index("incident_id", "action_code", Name = "uq_incident_actions_incident_code", IsUnique = true)]
public partial class IncidentAction
{
    [Key]
    public Guid IncidentActionId { get; set; }

    public Guid TenantId { get; set; }

    public Guid IncidentId { get; set; }

    [StringLength(50)]
    public string ActionCode { get; set; } = null!;

    [StringLength(200)]
    public string ActionTitle { get; set; } = null!;

    public string? ActionBody { get; set; }

    [StringLength(50)]
    public string ActionStatus { get; set; } = null!;

    public Guid? AssignedToUserId { get; set; }

    public DateTime? DueAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Guid? PerformedByUserId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("assigned_to_user_id")]
    [InverseProperty("incident_actionassigned_to_users")]
    public virtual User? AssignedToUser { get; set; }

    [ForeignKey("incident_id")]
    [InverseProperty("incident_actions")]
    public virtual Incident Incident { get; set; } = null!;

    [ForeignKey("performed_by_user_id")]
    [InverseProperty("incident_actionperformed_by_users")]
    public virtual User? PerformedByUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("incident_actions")]
    public virtual Tenant Tenant { get; set; } = null!;
}
