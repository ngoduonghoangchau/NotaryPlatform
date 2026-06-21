using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Rule catalog for dispatch logic.
/// </summary>
[Table("dispatch_rules", Schema = "operations")]
[Index("is_active", Name = "ix_dispatch_rules_is_active")]
[Index("priority", Name = "ix_dispatch_rules_priority")]
[Index("service_type_id", Name = "ix_dispatch_rules_service_type_id")]
[Index("tenant_id", Name = "ix_dispatch_rules_tenant_id")]
[Index("tenant_id", "rule_code", Name = "uq_dispatch_rules_tenant_code", IsUnique = true)]
public partial class DispatchRule
{
    [Key]
    public Guid DispatchRuleId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string RuleCode { get; set; } = null!;

    [StringLength(200)]
    public string RuleName { get; set; } = null!;

    public int Priority { get; set; }

    public bool IsActive { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public Guid? ServiceTypeId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Conditions { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Actions { get; set; } = null!;

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("dispatch_rules")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("dispatch_rulecreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("region_id")]
    [InverseProperty("dispatch_rules")]
    public virtual Region? Region { get; set; }

    [ForeignKey("service_type_id")]
    [InverseProperty("dispatch_rules")]
    public virtual ServiceType? ServiceType { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("dispatch_rules")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("dispatch_ruleupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class DispatchRule
{
    public DispatchRuleType rule_type { get; set; }
    public ServiceMode service_mode { get; set; }
}
// </auto-enum-partial>
