using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Recurring availability and blackout rules for notaries.
/// </summary>
[Table("notary_availability_rules", Schema = "operations")]
[Index("is_active", Name = "ix_notary_availability_rules_is_active")]
[Index("notary_id", Name = "ix_notary_availability_rules_notary_id")]
[Index("tenant_id", Name = "ix_notary_availability_rules_tenant_id")]
[Index("notary_id", "rule_code", Name = "uq_availability_rules_notary_code", IsUnique = true)]
public partial class NotaryAvailabilityRule
{
    [Key]
    public Guid AvailabilityRuleId { get; set; }

    public Guid TenantId { get; set; }

    public Guid NotaryId { get; set; }

    [StringLength(50)]
    public string RuleCode { get; set; } = null!;

    [StringLength(200)]
    public string RuleName { get; set; } = null!;

    public short DayOfWeek { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public DateOnly? EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("notary_availability_rules")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("notary_availability_rules")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("region_id")]
    [InverseProperty("notary_availability_rules")]
    public virtual Region? Region { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("notary_availability_rules")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class NotaryAvailabilityRule
{
    public AvailabilityRuleType rule_type { get; set; }
}
// </auto-enum-partial>
