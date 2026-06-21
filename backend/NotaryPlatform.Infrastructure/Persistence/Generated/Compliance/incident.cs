using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;

/// <summary>
/// Compliance incidents and violations.
/// </summary>
[Table("incidents", Schema = "compliance")]
[Index("detected_at", Name = "ix_incidents_detected_at")]
[Index("tenant_id", Name = "ix_incidents_tenant_id")]
[Index("tenant_id", "incident_code", Name = "uq_incidents_tenant_code", IsUnique = true)]
public partial class Incident
{
    [Key]
    public Guid IncidentId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string IncidentCode { get; set; } = null!;

    [StringLength(250)]
    public string Title { get; set; } = null!;

    public string? Summary { get; set; }

    public string? Details { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? JobId { get; set; }

    public Guid? ActId { get; set; }

    public Guid? JournalEntryId { get; set; }

    public Guid? SealId { get; set; }

    public Guid? CertificateId { get; set; }

    public DateTime DetectedAt { get; set; }

    public DateTime? ReportedAt { get; set; }

    public DateTime? ContainedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public Guid? ReportedByUserId { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public bool LegalHoldApplied { get; set; }

    public Guid? LegalHoldId { get; set; }

    public bool RegulatoryNotificationRequired { get; set; }

    public DateTime? RegulatoryNotifiedAt { get; set; }

    public string? ExternalReference { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("act_id")]
    [InverseProperty("incidents")]
    public virtual NotarialAct? Act { get; set; }

    [ForeignKey("assigned_to_user_id")]
    [InverseProperty("incidentassigned_to_users")]
    public virtual User? AssignedToUser { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("incidents")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("certificate_id")]
    [InverseProperty("incidents")]
    public virtual DigitalCertificate? Certificate { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("incidents")]
    public virtual Customer? Customer { get; set; }

    [InverseProperty("incident")]
    public virtual ICollection<IncidentAction> IncidentActions { get; set; } = new List<IncidentAction>();

    [ForeignKey("job_id")]
    [InverseProperty("incidents")]
    public virtual Job? Job { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("incidents")]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey("legal_hold_id")]
    [InverseProperty("incidents")]
    public virtual LegalHold? LegalHold { get; set; }

    [ForeignKey("region_id")]
    [InverseProperty("incidents")]
    public virtual Region? Region { get; set; }

    [ForeignKey("reported_by_user_id")]
    [InverseProperty("incidentreported_by_users")]
    public virtual User? ReportedByUser { get; set; }

    [ForeignKey("seal_id")]
    [InverseProperty("incidents")]
    public virtual Seal? Seal { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("incidents")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class Incident
{
    public IncidentStatus incident_status { get; set; }
    public IncidentType incident_type { get; set; }
    public IncidentSeverity severity { get; set; }
}
// </auto-enum-partial>
