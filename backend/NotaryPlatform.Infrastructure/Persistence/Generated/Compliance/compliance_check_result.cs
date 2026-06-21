using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;

/// <summary>
/// Detailed findings for compliance checks.
/// </summary>
[Table("compliance_check_results", Schema = "compliance")]
[Index("compliance_check_id", Name = "ix_compliance_check_results_check_id")]
[Index("entity_type", "entity_id", Name = "ix_compliance_check_results_entity")]
[Index("evaluated_at", Name = "ix_compliance_check_results_evaluated_at")]
[Index("tenant_id", Name = "ix_compliance_check_results_tenant_id")]
public partial class ComplianceCheckResult
{
    [Key]
    public Guid ComplianceCheckResultId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ComplianceCheckId { get; set; }

    [StringLength(50)]
    public string? RuleCode { get; set; }

    [StringLength(200)]
    public string? RuleName { get; set; }

    [StringLength(100)]
    public string? EntityType { get; set; }

    public Guid? EntityId { get; set; }

    [StringLength(100)]
    public string? FieldName { get; set; }

    public string? ExpectedValue { get; set; }

    public string? ActualValue { get; set; }

    public string? Message { get; set; }

    public string? Recommendation { get; set; }

    public bool IsBlocking { get; set; }

    public bool RequiresManualReview { get; set; }

    public DateTime EvaluatedAt { get; set; }

    public Guid? EvaluatedByUserId { get; set; }

    public Guid? EvaluatedByNotaryId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Evidence { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("compliance_check_id")]
    [InverseProperty("compliance_check_results")]
    public virtual ComplianceCheck ComplianceCheck { get; set; } = null!;

    [ForeignKey("evaluated_by_notary_id")]
    [InverseProperty("compliance_check_results")]
    public virtual Notary? EvaluatedByNotary { get; set; }

    [ForeignKey("evaluated_by_user_id")]
    [InverseProperty("compliance_check_results")]
    public virtual User? EvaluatedByUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("compliance_check_results")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class ComplianceCheckResult
{
    public CheckStatus result_status { get; set; }
    public CheckResultSeverity severity { get; set; }
}
// </auto-enum-partial>
