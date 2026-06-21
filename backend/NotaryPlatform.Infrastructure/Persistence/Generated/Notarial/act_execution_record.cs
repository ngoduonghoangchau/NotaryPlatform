using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;

/// <summary>
/// Execution-stage legal record, including oath and appearance confirmation.
/// </summary>
[Table("act_execution_records", Schema = "notarial")]
[Index("act_id", Name = "ix_act_execution_records_act_id")]
[Index("executed_by_notary_id", Name = "ix_act_execution_records_executed_by_notary_id")]
[Index("tenant_id", Name = "ix_act_execution_records_tenant_id")]
public partial class ActExecutionRecord
{
    [Key]
    public Guid ExecutionRecordId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ActId { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Guid? ExecutedByUserId { get; set; }

    public Guid ExecutedByNotaryId { get; set; }

    public bool PersonalAppearanceConfirmed { get; set; }

    public bool OathAdministered { get; set; }

    public string? OathText { get; set; }

    [StringLength(100)]
    public string? SignatureCaptureMethod { get; set; }

    public bool SignatureCaptured { get; set; }

    public DateTime? SignatureCapturedAt { get; set; }

    public string? Notes { get; set; }

    public string? Observations { get; set; }

    [Column(TypeName = "jsonb")]
    public string ExceptionFlags { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("act_id")]
    [InverseProperty("act_execution_records")]
    public virtual NotarialAct Act { get; set; } = null!;

    [ForeignKey("executed_by_notary_id")]
    [InverseProperty("act_execution_records")]
    public virtual Notary ExecutedByNotary { get; set; } = null!;

    [ForeignKey("executed_by_user_id")]
    [InverseProperty("act_execution_records")]
    public virtual User? ExecutedByUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("act_execution_records")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class ActExecutionRecord
{
    public ExecutionStatus execution_status { get; set; }
}
// </auto-enum-partial>
