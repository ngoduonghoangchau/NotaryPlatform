using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Candidate notaries per dispatch run.
/// </summary>
[Table("dispatch_candidates", Schema = "operations")]
[Index("dispatch_run_id", Name = "ix_dispatch_candidates_dispatch_run_id")]
[Index("notary_id", Name = "ix_dispatch_candidates_notary_id")]
[Index("rank_no", Name = "ix_dispatch_candidates_rank_no")]
[Index("tenant_id", Name = "ix_dispatch_candidates_tenant_id")]
public partial class DispatchCandidate
{
    [Key]
    public Guid DispatchCandidateId { get; set; }

    public Guid TenantId { get; set; }

    public Guid DispatchRunId { get; set; }

    public Guid NotaryId { get; set; }

    public int RankNo { get; set; }

    [Precision(12, 4)]
    public decimal? Score { get; set; }

    [Precision(10, 2)]
    public decimal? DistanceKm { get; set; }

    public int? TravelMinutes { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public Guid? ServiceTypeId { get; set; }

    public bool IsSelected { get; set; }

    public string? SelectionReason { get; set; }

    public string? RejectionReason { get; set; }

    [Column(TypeName = "jsonb")]
    public string ComplianceSnapshot { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string AvailabilitySnapshot { get; set; } = null!;

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("dispatch_candidates")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("dispatch_run_id")]
    [InverseProperty("dispatch_candidates")]
    public virtual DispatchRun DispatchRun { get; set; } = null!;

    [ForeignKey("notary_id")]
    [InverseProperty("dispatch_candidates")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("region_id")]
    [InverseProperty("dispatch_candidates")]
    public virtual Region? Region { get; set; }

    [ForeignKey("service_type_id")]
    [InverseProperty("dispatch_candidates")]
    public virtual ServiceType? ServiceType { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("dispatch_candidates")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class DispatchCandidate
{
    public DispatchCandidateStatus candidate_status { get; set; }
    public ServiceMode service_mode { get; set; }
}
// </auto-enum-partial>
