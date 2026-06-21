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
/// Compliance rule catalog used for validation and blocking.
/// </summary>
[Table("compliance_rules", Schema = "compliance")]
[Index("is_active", Name = "ix_compliance_rules_is_active")]
[Index("state_code", Name = "ix_compliance_rules_state_code")]
[Index("tenant_id", Name = "ix_compliance_rules_tenant_id")]
[Index("tenant_id", "rule_code", Name = "uq_compliance_rules_tenant_code", IsUnique = true)]
public partial class ComplianceRule
{
    [Key]
    public Guid ComplianceRuleId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string RuleCode { get; set; } = null!;

    [StringLength(200)]
    public string RuleName { get; set; } = null!;

    [StringLength(2)]
    public string? StateCode { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public Guid? NotaryId { get; set; }

    public Guid? RoleId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? ServiceTypeId { get; set; }

    public Guid? JobId { get; set; }

    public Guid? ActId { get; set; }

    public Guid? JournalEntryId { get; set; }

    public Guid? SealId { get; set; }

    public Guid? CertificateId { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(100)]
    public string? Subcategory { get; set; }

    [StringLength(250)]
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Rationale { get; set; }

    public string ConditionExpression { get; set; } = null!;

    public string? ActionExpression { get; set; }

    public bool BlockOnFailure { get; set; }

    public bool RequiresManualReview { get; set; }

    public bool IsMandatory { get; set; }

    public bool IsActive { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("act_id")]
    [InverseProperty("compliance_rules")]
    public virtual NotarialAct? Act { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("compliance_rules")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("certificate_id")]
    [InverseProperty("compliance_rules")]
    public virtual DigitalCertificate? Certificate { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("compliance_rulecreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("compliance_rules")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("job_id")]
    [InverseProperty("compliance_rules")]
    public virtual Job? Job { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("compliance_rules")]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("compliance_rules")]
    public virtual Notary? Notary { get; set; }

    [ForeignKey("region_id")]
    [InverseProperty("compliance_rules")]
    public virtual Region? Region { get; set; }

    [ForeignKey("role_id")]
    [InverseProperty("compliance_rules")]
    public virtual Role? Role { get; set; }

    [ForeignKey("seal_id")]
    [InverseProperty("compliance_rules")]
    public virtual Seal? Seal { get; set; }

    [ForeignKey("service_type_id")]
    [InverseProperty("compliance_rules")]
    public virtual ServiceType? ServiceType { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("compliance_rules")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("compliance_ruleupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class ComplianceRule
{
    public RuleStatus rule_status { get; set; }
    public RuleScope scope { get; set; }
    public RuleSeverity severity { get; set; }
}
// </auto-enum-partial>
