using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Journal;

/// <summary>
/// State-based retention rules for journal entries.
/// </summary>
[Table("journal_retention_policies", Schema = "journal")]
[Index("is_active", Name = "ix_journal_retention_policies_is_active")]
[Index("state_code", Name = "ix_journal_retention_policies_state_code")]
[Index("tenant_id", Name = "ix_journal_retention_policies_tenant_id")]
[Index("tenant_id", "policy_code", Name = "uq_journal_retention_policies_tenant_code", IsUnique = true)]
public partial class JournalRetentionPolicy
{
    [Key]
    public Guid RetentionPolicyId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string PolicyCode { get; set; } = null!;

    [StringLength(200)]
    public string PolicyName { get; set; } = null!;

    [StringLength(2)]
    public string StateCode { get; set; } = null!;

    public int RetentionYears { get; set; }

    public bool IsLegalHoldEligible { get; set; }

    public bool ExportAllowed { get; set; }

    public bool TransferAllowed { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("journal_retention_policycreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("journal_retention_policies")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("journal_retention_policyupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}
