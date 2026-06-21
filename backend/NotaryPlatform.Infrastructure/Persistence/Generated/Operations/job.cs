using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Operations;

/// <summary>
/// Confirmed operational job records.
/// </summary>
[Table("jobs", Schema = "operations")]
[Index("customer_id", Name = "ix_jobs_customer_id")]
[Index("job_request_id", Name = "ix_jobs_job_request_id")]
[Index("scheduled_start_at", Name = "ix_jobs_scheduled_start_at")]
[Index("service_type_id", Name = "ix_jobs_service_type_id")]
[Index("sla_due_at", Name = "ix_jobs_sla_due_at")]
[Index("tenant_id", Name = "ix_jobs_tenant_id")]
[Index("tenant_id", "job_code", Name = "uq_jobs_tenant_code", IsUnique = true)]
public partial class Job
{
    [Key]
    public Guid JobId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string JobCode { get; set; } = null!;

    public Guid? JobRequestId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? CustomerContactId { get; set; }

    public Guid ServiceTypeId { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    public DateTime? RequestedStartAt { get; set; }

    public DateTime? RequestedEndAt { get; set; }

    public DateTime? ScheduledStartAt { get; set; }

    public DateTime? ScheduledEndAt { get; set; }

    public DateTime? ActualStartAt { get; set; }

    public DateTime? ActualEndAt { get; set; }

    [StringLength(64)]
    public string Timezone { get; set; } = null!;

    public int? EstimatedDurationMinutes { get; set; }

    public int? ActualDurationMinutes { get; set; }

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

    public bool RushFlag { get; set; }

    public bool LockRequired { get; set; }

    public DateTime? LockedAt { get; set; }

    public Guid? LockedByUserId { get; set; }

    public string? LockedReason { get; set; }

    public DateTime? CancelledAt { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public string? CancelReason { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? FailedAt { get; set; }

    public string? FailureReason { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("jobs")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("cancelled_by_user_id")]
    [InverseProperty("jobcancelled_by_users")]
    public virtual User? CancelledByUser { get; set; }

    [InverseProperty("job")]
    public virtual ICollection<CommunicationReminder> CommunicationReminders { get; set; } = new List<CommunicationReminder>();

    [InverseProperty("job")]
    public virtual ICollection<CommunicationThread> CommunicationThreads { get; set; } = new List<CommunicationThread>();

    [InverseProperty("job")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [InverseProperty("job")]
    public virtual ICollection<ComplianceRule> ComplianceRules { get; set; } = new List<ComplianceRule>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("jobcreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("jobs")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("customer_contact_id")]
    [InverseProperty("jobs")]
    public virtual CustomerContact? CustomerContact { get; set; }

    [InverseProperty("job")]
    public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    [InverseProperty("job")]
    public virtual ICollection<InternalNote> InternalNotes { get; set; } = new List<InternalNote>();

    [InverseProperty("job")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    [InverseProperty("job")]
    public virtual JobAssignment? JobAssignment { get; set; }

    [InverseProperty("job")]
    public virtual ICollection<JobReminder> JobReminders { get; set; } = new List<JobReminder>();

    [ForeignKey("job_request_id")]
    [InverseProperty("jobs")]
    public virtual JobRequest? JobRequest { get; set; }

    [InverseProperty("job")]
    public virtual ICollection<JobStatusHistory> JobStatusHistories { get; set; } = new List<JobStatusHistory>();

    [InverseProperty("job")]
    public virtual ICollection<JobTimelineEvent> JobTimelineEvents { get; set; } = new List<JobTimelineEvent>();

    [InverseProperty("job")]
    public virtual ICollection<LegalHold> LegalHolds { get; set; } = new List<LegalHold>();

    [ForeignKey("locked_by_user_id")]
    [InverseProperty("joblocked_by_users")]
    public virtual User? LockedByUser { get; set; }

    [InverseProperty("job")]
    public virtual ICollection<NotarialAct> NotarialActs { get; set; } = new List<NotarialAct>();

    [ForeignKey("region_id")]
    [InverseProperty("jobs")]
    public virtual Region? Region { get; set; }

    [InverseProperty("job")]
    public virtual ICollection<RevenueShare> RevenueShares { get; set; } = new List<RevenueShare>();

    [InverseProperty("job")]
    public virtual ICollection<ScheduleBlock> ScheduleBlocks { get; set; } = new List<ScheduleBlock>();

    [ForeignKey("service_type_id")]
    [InverseProperty("jobs")]
    public virtual ServiceType ServiceType { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("jobs")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("jobupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class Job
{
    public JobPriority priority { get; set; }
    public ServiceMode service_mode { get; set; }
    public JobStatus status { get; set; }
}
// </auto-enum-partial>
