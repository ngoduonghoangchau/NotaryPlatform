using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Operational service catalog used by requests, jobs, and dispatch.
/// </summary>
[Table("service_types", Schema = "operations")]
[Index("is_active", Name = "ix_service_types_is_active")]
[Index("tenant_id", Name = "ix_service_types_tenant_id")]
[Index("tenant_id", "service_code", Name = "uq_service_types_tenant_code", IsUnique = true)]
public partial class ServiceType
{
    [Key]
    public Guid ServiceTypeId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string ServiceCode { get; set; } = null!;

    [StringLength(200)]
    public string ServiceName { get; set; } = null!;

    [StringLength(100)]
    public string? Category { get; set; }

    public string? Description { get; set; }

    public int? DefaultDurationMinutes { get; set; }

    public bool RequiresIdentityVerification { get; set; }

    public bool RequiresJournalEntry { get; set; }

    public bool RequiresSeal { get; set; }

    public bool RequiresSignature { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "jsonb")]
    public string Settings { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("service_type")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [InverseProperty("service_type")]
    public virtual ICollection<ComplianceRule> ComplianceRules { get; set; } = new List<ComplianceRule>();

    [InverseProperty("service_type")]
    public virtual ICollection<DispatchCandidate> DispatchCandidates { get; set; } = new List<DispatchCandidate>();

    [InverseProperty("service_type")]
    public virtual ICollection<DispatchRule> DispatchRules { get; set; } = new List<DispatchRule>();

    [InverseProperty("service_type")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    [InverseProperty("service_type")]
    public virtual ICollection<JobRequest> JobRequests { get; set; } = new List<JobRequest>();

    [InverseProperty("service_type")]
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    [InverseProperty("service_type")]
    public virtual ICollection<SealAccessPolicy> SealAccessPolicies { get; set; } = new List<SealAccessPolicy>();

    [ForeignKey("tenant_id")]
    [InverseProperty("service_types")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class ServiceType
{
    public ServiceMode default_mode { get; set; }
}
// </auto-enum-partial>
