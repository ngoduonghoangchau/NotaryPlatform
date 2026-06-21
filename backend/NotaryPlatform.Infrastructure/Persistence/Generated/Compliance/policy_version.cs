using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;

/// <summary>
/// Versioned compliance policies and rule bundles.
/// </summary>
[Table("policy_versions", Schema = "compliance")]
[Index("published_at", Name = "ix_policy_versions_published_at")]
[Index("state_code", Name = "ix_policy_versions_state_code")]
[Index("tenant_id", Name = "ix_policy_versions_tenant_id")]
[Index("tenant_id", "policy_code", "version_no", Name = "uq_policy_versions_tenant_code_version", IsUnique = true)]
public partial class PolicyVersion
{
    [Key]
    public Guid PolicyVersionId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string PolicyCode { get; set; } = null!;

    [StringLength(200)]
    public string PolicyName { get; set; } = null!;

    public int VersionNo { get; set; }

    public Guid? ParentPolicyVersionId { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime? DeprecatedAt { get; set; }

    public string? ChangeSummary { get; set; }

    public string PolicyBody { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string PolicyJson { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("parent_policy_version")]
    public virtual ICollection<PolicyVersion> InverseparentPolicyVersion { get; set; } = new List<PolicyVersion>();

    [ForeignKey("branch_id")]
    [InverseProperty("policy_versions")]
    public virtual Branch? Branch { get; set; }

    [InverseProperty("policy_version")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("policy_versioncreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("parent_policy_version_id")]
    [InverseProperty("Inverseparent_policy_version")]
    public virtual PolicyVersion? ParentPolicyVersion { get; set; }

    [ForeignKey("region_id")]
    [InverseProperty("policy_versions")]
    public virtual Region? Region { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("policy_versions")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("policy_versionupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class PolicyVersion
{
    public PolicyStatus policy_status { get; set; }
    public RuleScope scope { get; set; }
}
// </auto-enum-partial>
