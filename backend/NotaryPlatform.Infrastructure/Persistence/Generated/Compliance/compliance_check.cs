using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;

/// <summary>
/// Compliance evaluation runs.
/// </summary>
[Table("compliance_checks", Schema = "compliance")]
[Index("policy_version_id", Name = "ix_compliance_checks_policy_version_id")]
[Index("started_at", Name = "ix_compliance_checks_started_at")]
[Index("state_code", Name = "ix_compliance_checks_state_code")]
[Index("tenant_id", Name = "ix_compliance_checks_tenant_id")]
[Index("tenant_id", "check_code", Name = "uq_compliance_checks_tenant_code", IsUnique = true)]
public partial class ComplianceCheck
{
    [Key]
    public Guid ComplianceCheckId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string CheckCode { get; set; } = null!;

    [StringLength(200)]
    public string CheckName { get; set; } = null!;

    [StringLength(50)]
    public string? RuleBundleCode { get; set; }

    public Guid? PolicyVersionId { get; set; }

    public Guid? TriggeredByUserId { get; set; }

    public Guid? TriggeredByNotaryId { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? ServiceTypeId { get; set; }

    public Guid? JobId { get; set; }

    public Guid? ActId { get; set; }

    public Guid? JournalEntryId { get; set; }

    public Guid? SealId { get; set; }

    public Guid? CertificateId { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? BlockedAt { get; set; }

    public DateTime? ManualReviewAt { get; set; }

    public string? BlockReason { get; set; }

    public string? Summary { get; set; }

    [Column(TypeName = "jsonb")]
    public string ResultSummary { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("act_id")]
    [InverseProperty("compliance_checks")]
    public virtual NotarialAct? Act { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("compliance_checks")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("certificate_id")]
    [InverseProperty("compliance_checks")]
    public virtual DigitalCertificate? Certificate { get; set; }

    [InverseProperty("compliance_check")]
    public virtual ICollection<ComplianceCheckResult> ComplianceCheckResults { get; set; } = new List<ComplianceCheckResult>();

    [ForeignKey("customer_id")]
    [InverseProperty("compliance_checks")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("job_id")]
    [InverseProperty("compliance_checks")]
    public virtual Job? Job { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("compliance_checks")]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey("policy_version_id")]
    [InverseProperty("compliance_checks")]
    public virtual PolicyVersion? PolicyVersion { get; set; }

    [ForeignKey("region_id")]
    [InverseProperty("compliance_checks")]
    public virtual Region? Region { get; set; }

    [ForeignKey("seal_id")]
    [InverseProperty("compliance_checks")]
    public virtual Seal? Seal { get; set; }

    [ForeignKey("service_type_id")]
    [InverseProperty("compliance_checks")]
    public virtual ServiceType? ServiceType { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("compliance_checks")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("triggered_by_notary_id")]
    [InverseProperty("compliance_checks")]
    public virtual Notary? TriggeredByNotary { get; set; }

    [ForeignKey("triggered_by_user_id")]
    [InverseProperty("compliance_checks")]
    public virtual User? TriggeredByUser { get; set; }
}

// <auto-enum-partial>
public partial class ComplianceCheck
{
    public CheckStatus check_status { get; set; }
}
// </auto-enum-partial>
