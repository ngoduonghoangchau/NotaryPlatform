using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// Security incidents, compromises, unauthorized use, and response lifecycle.
/// </summary>
[Table("security_incidents", Schema = "security")]
[Index("detected_at", Name = "ix_security_incidents_detected_at")]
[Index("tenant_id", Name = "ix_security_incidents_tenant_id")]
[Index("tenant_id", "incident_code", Name = "uq_security_incidents_tenant_code", IsUnique = true)]
public partial class SecurityIncident
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

    public DateTime DetectedAt { get; set; }

    public DateTime? ReportedAt { get; set; }

    public DateTime? ContainedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public Guid? ReportedByUserId { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public Guid? PrimaryNotaryId { get; set; }

    public Guid? AffectedSealId { get; set; }

    public Guid? AffectedCertificateId { get; set; }

    public Guid? AffectedDeviceId { get; set; }

    public bool LegalHold { get; set; }

    public bool RegulatoryNotificationRequired { get; set; }

    public DateTime? RegulatoryNotifiedAt { get; set; }

    public string? ExternalReference { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("affected_certificate_id")]
    [InverseProperty("security_incidents")]
    public virtual DigitalCertificate? AffectedCertificate { get; set; }

    [ForeignKey("affected_device_id")]
    [InverseProperty("security_incidents")]
    public virtual TrustedDevice? AffectedDevice { get; set; }

    [ForeignKey("affected_seal_id")]
    [InverseProperty("security_incidents")]
    public virtual Seal? AffectedSeal { get; set; }

    [ForeignKey("assigned_to_user_id")]
    [InverseProperty("security_incidentassigned_to_users")]
    public virtual User? AssignedToUser { get; set; }

    [InverseProperty("incident")]
    public virtual ICollection<CommunicationThread> CommunicationThreads { get; set; } = new List<CommunicationThread>();

    [InverseProperty("incident")]
    public virtual ICollection<EmergencyLock> EmergencyLocks { get; set; } = new List<EmergencyLock>();

    [InverseProperty("incident")]
    public virtual ICollection<InternalNote> InternalNotes { get; set; } = new List<InternalNote>();

    [InverseProperty("incident")]
    public virtual ICollection<LegalHold> LegalHolds { get; set; } = new List<LegalHold>();

    [ForeignKey("primary_notary_id")]
    [InverseProperty("security_incidents")]
    public virtual Notary? PrimaryNotary { get; set; }

    [ForeignKey("reported_by_user_id")]
    [InverseProperty("security_incidentreported_by_users")]
    public virtual User? ReportedByUser { get; set; }

    [InverseProperty("incident")]
    public virtual ICollection<SealReplacement> SealReplacements { get; set; } = new List<SealReplacement>();

    [InverseProperty("incident")]
    public virtual ICollection<SealRevocation> SealRevocations { get; set; } = new List<SealRevocation>();

    [InverseProperty("incident")]
    public virtual ICollection<SealUsageLog> SealUsageLogs { get; set; } = new List<SealUsageLog>();

    [InverseProperty("incident")]
    public virtual ICollection<SecurityIncidentAction> SecurityIncidentActions { get; set; } = new List<SecurityIncidentAction>();

    [ForeignKey("tenant_id")]
    [InverseProperty("security_incidents")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class SecurityIncident
{
    public IncidentType incident_type { get; set; }
    public IncidentSeverity severity { get; set; }
    public IncidentStatus status { get; set; }
}
// </auto-enum-partial>
