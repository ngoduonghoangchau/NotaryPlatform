using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;

/// <summary>
/// Catalog of allowed void reasons.
/// </summary>
[Table("void_reasons", Schema = "notarial")]
[Index("tenant_id", Name = "ix_void_reasons_tenant_id")]
[Index("tenant_id", "reason_code", Name = "uq_void_reasons_tenant_code", IsUnique = true)]
public partial class VoidReason
{
    [Key]
    public Guid VoidReasonId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string ReasonCode { get; set; } = null!;

    [StringLength(200)]
    public string ReasonName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("void_reasons")]
    public virtual Tenant Tenant { get; set; } = null!;
}
