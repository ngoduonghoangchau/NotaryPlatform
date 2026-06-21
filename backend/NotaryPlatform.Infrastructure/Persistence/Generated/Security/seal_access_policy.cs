using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// Who may use a seal or certificate, under what conditions, and with what approvals.
/// </summary>
[Table("seal_access_policies", Schema = "security")]
[Index("branch_id", Name = "ix_policies_branch_id")]
[Index("region_id", Name = "ix_policies_region_id")]
[Index("state_code", Name = "ix_policies_state_code")]
[Index("tenant_id", Name = "ix_policies_tenant_id")]
[Index("tenant_id", "policy_code", Name = "uq_policies_tenant_code", IsUnique = true)]
public partial class SealAccessPolicy
{
    [Key]
    public Guid PolicyId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string PolicyCode { get; set; } = null!;

    [StringLength(200)]
    public string PolicyName { get; set; } = null!;

    [StringLength(2)]
    public string? StateCode { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public Guid? NotaryId { get; set; }

    public Guid? ServiceTypeId { get; set; }

    [Column(TypeName = "jsonb")]
    public string RequiredRoles { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string RequiredPermissions { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Conditions { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string ApprovalWorkflow { get; set; } = null!;

    public bool MfaRequired { get; set; }

    public bool ApprovalRequired { get; set; }

    public bool DelegationAllowed { get; set; }

    public bool EmergencyOverrideAllowed { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("seal_access_policies")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("seal_access_policycreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("seal_access_policies")]
    public virtual Notary? Notary { get; set; }

    [ForeignKey("region_id")]
    [InverseProperty("seal_access_policies")]
    public virtual Region? Region { get; set; }

    [InverseProperty("policy")]
    public virtual ICollection<SealUsageLog> SealUsageLogs { get; set; } = new List<SealUsageLog>();

    [ForeignKey("service_type_id")]
    [InverseProperty("seal_access_policies")]
    public virtual ServiceType? ServiceType { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("seal_access_policies")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("seal_access_policyupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class SealAccessPolicy
{
    public PolicyScope scope { get; set; }
    public PolicyStatus status { get; set; }
    public PolicyTargetType target_type { get; set; }
}
// </auto-enum-partial>
