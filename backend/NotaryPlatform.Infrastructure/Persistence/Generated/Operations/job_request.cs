using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Incoming job requests and orders.
/// </summary>
[Table("job_requests", Schema = "operations")]
[Index("customer_id", Name = "ix_job_requests_customer_id")]
[Index("requested_start_at", Name = "ix_job_requests_requested_start_at")]
[Index("service_type_id", Name = "ix_job_requests_service_type_id")]
[Index("sla_due_at", Name = "ix_job_requests_sla_due_at")]
[Index("tenant_id", Name = "ix_job_requests_tenant_id")]
[Index("tenant_id", "request_code", Name = "uq_job_requests_tenant_code", IsUnique = true)]
public partial class JobRequest
{
    [Key]
    public Guid JobRequestId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string RequestCode { get; set; } = null!;

    public Guid CustomerId { get; set; }

    public Guid? CustomerContactId { get; set; }

    public Guid? CreatedByUserId { get; set; }

    [StringLength(50)]
    public string SourceChannel { get; set; } = null!;

    public Guid ServiceTypeId { get; set; }

    public DateTime? RequestedStartAt { get; set; }

    public DateTime? RequestedEndAt { get; set; }

    public DateOnly? PreferredDate { get; set; }

    [StringLength(64)]
    public string Timezone { get; set; } = null!;

    public int? EstimatedDurationMinutes { get; set; }

    public bool RushFlag { get; set; }

    public string? SpecialInstructions { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    [StringLength(200)]
    public string? LocationName { get; set; }

    [StringLength(200)]
    public string? AddressLine1 { get; set; }

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(2)]
    public string? CountryCode { get; set; }

    [Precision(10, 7)]
    public decimal? Latitude { get; set; }

    [Precision(10, 7)]
    public decimal? Longitude { get; set; }

    public string? VirtualMeetingUrl { get; set; }

    public DateTime? SlaDueAt { get; set; }

    [Precision(18, 2)]
    public decimal? QuotedAmount { get; set; }

    [StringLength(3)]
    public string QuoteCurrencyCode { get; set; } = null!;

    public Guid? AssignedDispatcherUserId { get; set; }

    public DateTime? TriagedAt { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public string? CancelReason { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("assigned_dispatcher_user_id")]
    [InverseProperty("job_requestassigned_dispatcher_users")]
    public virtual User? AssignedDispatcherUser { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("job_requests")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("cancelled_by_user_id")]
    [InverseProperty("job_requestcancelled_by_users")]
    public virtual User? CancelledByUser { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("job_requestcreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("job_requests")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("customer_contact_id")]
    [InverseProperty("job_requests")]
    public virtual CustomerContact? CustomerContact { get; set; }

    [InverseProperty("job_request")]
    public virtual ICollection<DispatchRun> DispatchRuns { get; set; } = new List<DispatchRun>();

    [InverseProperty("job_request")]
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    [InverseProperty("job_request")]
    public virtual ICollection<NotarialAct> NotarialActs { get; set; } = new List<NotarialAct>();

    [ForeignKey("region_id")]
    [InverseProperty("job_requests")]
    public virtual Region? Region { get; set; }

    [ForeignKey("service_type_id")]
    [InverseProperty("job_requests")]
    public virtual ServiceType ServiceType { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("job_requests")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class JobRequest
{
    public JobPriority priority { get; set; }
    public ServiceMode service_mode { get; set; }
    public JobRequestStatus status { get; set; }
}
// </auto-enum-partial>
