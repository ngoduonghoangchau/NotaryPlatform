using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Service-level agreements linked to contracts.
/// </summary>
[Table("sla_agreements", Schema = "crm")]
[Index("contract_id", Name = "ix_sla_agreements_contract_id")]
[Index("tenant_id", Name = "ix_sla_agreements_tenant_id")]
[Index("contract_id", "sla_code", Name = "uq_sla_agreements_contract_code", IsUnique = true)]
public partial class SlaAgreement
{
    [Key]
    public Guid SlaId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ContractId { get; set; }

    [StringLength(50)]
    public string SlaCode { get; set; } = null!;

    [StringLength(200)]
    public string SlaName { get; set; } = null!;

    public int? TargetTurnaroundMinutes { get; set; }

    [Precision(5, 2)]
    public decimal? OnTimeTargetPercent { get; set; }

    public string? PenaltyTerms { get; set; }

    public string? ServiceHours { get; set; }

    public int? ResponseTimeMinutes { get; set; }

    public string? EscalationRules { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("contract_id")]
    [InverseProperty("sla_agreements")]
    public virtual Contract Contract { get; set; } = null!;

    [ForeignKey("created_by_user_id")]
    [InverseProperty("sla_agreementcreated_by_users")]
    public virtual User CreatedByUser { get; set; } = null!;

    [InverseProperty("sla")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [ForeignKey("tenant_id")]
    [InverseProperty("sla_agreements")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("sla_agreementupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class SlaAgreement
{
    public SlaStatus status { get; set; }
}
// </auto-enum-partial>
