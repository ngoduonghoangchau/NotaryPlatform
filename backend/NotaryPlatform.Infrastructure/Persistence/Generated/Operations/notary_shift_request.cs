using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Time-off, shift change, and blackout requests.
/// </summary>
[Table("notary_shift_requests", Schema = "operations")]
[Index("notary_id", Name = "ix_notary_shift_requests_notary_id")]
[Index("start_at", Name = "ix_notary_shift_requests_start_at")]
[Index("tenant_id", Name = "ix_notary_shift_requests_tenant_id")]
[Index("notary_id", "request_code", Name = "uq_shift_requests_notary_code", IsUnique = true)]
public partial class NotaryShiftRequest
{
    [Key]
    public Guid ShiftRequestId { get; set; }

    public Guid TenantId { get; set; }

    public Guid NotaryId { get; set; }

    [StringLength(50)]
    public string RequestCode { get; set; } = null!;

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public string? Reason { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public Guid? RequestedByUserId { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? RejectedAt { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("approved_by_user_id")]
    [InverseProperty("notary_shift_requestapproved_by_users")]
    public virtual User? ApprovedByUser { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("notary_shift_requests")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("notary_shift_requests")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("region_id")]
    [InverseProperty("notary_shift_requests")]
    public virtual Region? Region { get; set; }

    [ForeignKey("requested_by_user_id")]
    [InverseProperty("notary_shift_requestrequested_by_users")]
    public virtual User? RequestedByUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("notary_shift_requests")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class NotaryShiftRequest
{
    public AvailabilityRuleType request_type { get; set; }
    public ShiftRequestStatus status { get; set; }
}
// </auto-enum-partial>
