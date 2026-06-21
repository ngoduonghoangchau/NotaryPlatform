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
/// Legal hold records for preservation and blocking.
/// </summary>
[Table("legal_holds", Schema = "compliance")]
[Index("applied_at", Name = "ix_legal_holds_applied_at")]
[Index("state_code", Name = "ix_legal_holds_state_code")]
[Index("tenant_id", Name = "ix_legal_holds_tenant_id")]
[Index("tenant_id", "hold_code", Name = "uq_legal_holds_tenant_code", IsUnique = true)]
public partial class LegalHold
{
    [Key]
    public Guid LegalHoldId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string HoldCode { get; set; } = null!;

    [StringLength(200)]
    public string HoldName { get; set; } = null!;

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

    public Guid? IncidentId { get; set; }

    public string Reason { get; set; } = null!;

    public string? HoldDetails { get; set; }

    public DateTime AppliedAt { get; set; }

    public DateTime? ReleasedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public string? ReleaseReason { get; set; }

    public Guid AppliedByUserId { get; set; }

    public Guid? ReleasedByUserId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("act_id")]
    [InverseProperty("legal_holds")]
    public virtual NotarialAct? Act { get; set; }

    [ForeignKey("applied_by_user_id")]
    [InverseProperty("legal_holdapplied_by_users")]
    public virtual User AppliedByUser { get; set; } = null!;

    [ForeignKey("branch_id")]
    [InverseProperty("legal_holds")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("certificate_id")]
    [InverseProperty("legal_holds")]
    public virtual DigitalCertificate? Certificate { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("legal_holds")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("incident_id")]
    [InverseProperty("legal_holds")]
    public virtual SecurityIncident? Incident { get; set; }

    [InverseProperty("legal_hold")]
    public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    [InverseProperty("legal_hold")]
    public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();

    [ForeignKey("job_id")]
    [InverseProperty("legal_holds")]
    public virtual Job? Job { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("legal_holds")]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey("region_id")]
    [InverseProperty("legal_holds")]
    public virtual Region? Region { get; set; }

    [ForeignKey("released_by_user_id")]
    [InverseProperty("legal_holdreleased_by_users")]
    public virtual User? ReleasedByUser { get; set; }

    [ForeignKey("seal_id")]
    [InverseProperty("legal_holds")]
    public virtual Seal? Seal { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("legal_holds")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class LegalHold
{
    public RuleScope hold_scope { get; set; }
    public LegalHoldStatus hold_status { get; set; }
}
// </auto-enum-partial>
