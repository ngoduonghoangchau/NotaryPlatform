using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;

/// <summary>
/// Retention rules for records and artifacts.
/// </summary>
[Table("retention_policies", Schema = "compliance")]
[Index("applies_to_entity_type", Name = "ix_retention_policies_entity_type")]
[Index("state_code", Name = "ix_retention_policies_state_code")]
[Index("tenant_id", Name = "ix_retention_policies_tenant_id")]
[Index("tenant_id", "policy_code", Name = "uq_retention_policies_tenant_code", IsUnique = true)]
public partial class RetentionPolicy
{
    [Key]
    public Guid RetentionPolicyId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string PolicyCode { get; set; } = null!;

    [StringLength(200)]
    public string PolicyName { get; set; } = null!;

    [StringLength(2)]
    public string? StateCode { get; set; }

    [StringLength(100)]
    public string AppliesToEntityType { get; set; } = null!;

    public int RetentionYears { get; set; }

    public bool DestroyAfterRetention { get; set; }

    public bool LegalHoldEligible { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("retention_policycreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [InverseProperty("retention_policy")]
    public virtual ICollection<RetentionJob> RetentionJobs { get; set; } = new List<RetentionJob>();

    [ForeignKey("tenant_id")]
    [InverseProperty("retention_policies")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("retention_policyupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class RetentionPolicy
{
    public PolicyStatus policy_status { get; set; }
    public RuleScope scope { get; set; }
}
// </auto-enum-partial>
