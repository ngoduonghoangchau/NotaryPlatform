using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;
using NotaryPlatform.Domain.Features.Identity.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Identity;

/// <summary>
/// Central notary profile linked to application user.
/// </summary>
[Table("notaries", Schema = "identity")]
[Index("branch_id", Name = "ix_notaries_branch_id")]
[Index("commissioning_state_code", Name = "ix_notaries_commissioning_state_code")]
[Index("region_id", Name = "ix_notaries_region_id")]
[Index("tenant_id", Name = "ix_notaries_tenant_id")]
[Index("user_id", Name = "ix_notaries_user_id")]
[Index("tenant_id", "internal_notary_code", Name = "uq_notaries_tenant_code", IsUnique = true)]
[Index("user_id", Name = "uq_notaries_user", IsUnique = true)]
public partial class Notary
{
    [Key]
    public Guid NotaryId { get; set; }

    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    [StringLength(50)]
    public string InternalNotaryCode { get; set; } = null!;

    [StringLength(200)]
    public string? PublicDisplayName { get; set; }

    [StringLength(2)]
    public string CommissioningStateCode { get; set; } = null!;

    [StringLength(100)]
    public string? CommissionNumber { get; set; }

    public DateOnly? CommissionIssueDate { get; set; }

    public DateOnly? CommissionExpirationDate { get; set; }

    [StringLength(50)]
    public string? EmploymentType { get; set; }

    public DateOnly? StartDate { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    [Precision(5, 2)]
    public decimal ErrorRate { get; set; }

    [Precision(3, 2)]
    public decimal CustomerRating { get; set; }

    public int TotalJobsCompleted { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Settings { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("actor_notary")]
    public virtual ICollection<ActAuditLog> ActAuditLogs { get; set; } = new List<ActAuditLog>();

    [InverseProperty("executed_by_notary")]
    public virtual ICollection<ActExecutionRecord> ActExecutionRecords { get; set; } = new List<ActExecutionRecord>();

    [InverseProperty("verified_by_notary")]
    public virtual ICollection<ActIdentityVerification> ActIdentityVerifications { get; set; } = new List<ActIdentityVerification>();

    [InverseProperty("changed_by_notary")]
    public virtual ICollection<ActStatusHistory> ActStatusHistories { get; set; } = new List<ActStatusHistory>();

    [ForeignKey("branch_id")]
    [InverseProperty("notaries")]
    public virtual Branch? Branch { get; set; }

    [InverseProperty("callee_notary")]
    public virtual ICollection<CallLog> CallLogcalleeNotaries { get; set; } = new List<CallLog>();

    [InverseProperty("caller_notary")]
    public virtual ICollection<CallLog> CallLogcallerNotaries { get; set; } = new List<CallLog>();

    [InverseProperty("actor_notary")]
    public virtual ICollection<CommunicationAuditLog> CommunicationAuditLogs { get; set; } = new List<CommunicationAuditLog>();

    [InverseProperty("sender_notary")]
    public virtual ICollection<CommunicationMessage> CommunicationMessages { get; set; } = new List<CommunicationMessage>();

    [InverseProperty("notary")]
    public virtual ICollection<CommunicationParticipant> CommunicationParticipants { get; set; } = new List<CommunicationParticipant>();

    [InverseProperty("recipient_notary")]
    public virtual ICollection<CommunicationReminder> CommunicationReminders { get; set; } = new List<CommunicationReminder>();

    [InverseProperty("actor_notary")]
    public virtual ICollection<ComplianceAuditEvent> ComplianceAuditEvents { get; set; } = new List<ComplianceAuditEvent>();

    [InverseProperty("actor_notary")]
    public virtual ICollection<ComplianceAuditLog> ComplianceAuditLogs { get; set; } = new List<ComplianceAuditLog>();

    [InverseProperty("evaluated_by_notary")]
    public virtual ICollection<ComplianceCheckResult> ComplianceCheckResults { get; set; } = new List<ComplianceCheckResult>();

    [InverseProperty("triggered_by_notary")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [InverseProperty("notary")]
    public virtual ICollection<ComplianceRule> ComplianceRules { get; set; } = new List<ComplianceRule>();

    [InverseProperty("notary")]
    public virtual ICollection<DigitalCertificate> DigitalCertificates { get; set; } = new List<DigitalCertificate>();

    [InverseProperty("notary")]
    public virtual ICollection<DispatchCandidate> DispatchCandidates { get; set; } = new List<DispatchCandidate>();

    [InverseProperty("notary")]
    public virtual ICollection<JobAssignment> JobAssignments { get; set; } = new List<JobAssignment>();

    [InverseProperty("recipient_notary")]
    public virtual ICollection<JobReminder> JobReminders { get; set; } = new List<JobReminder>();

    [InverseProperty("related_notary")]
    public virtual ICollection<JobTimelineEvent> JobTimelineEvents { get; set; } = new List<JobTimelineEvent>();

    [InverseProperty("actor_notary")]
    public virtual ICollection<JournalAuditLog> JournalAuditLogs { get; set; } = new List<JournalAuditLog>();

    [InverseProperty("notary")]
    public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();

    [InverseProperty("notary")]
    public virtual ICollection<NotarialAct> NotarialActs { get; set; } = new List<NotarialAct>();

    [InverseProperty("notary")]
    public virtual ICollection<NotaryAuditNote> NotaryAuditNotes { get; set; } = new List<NotaryAuditNote>();

    [InverseProperty("notary")]
    public virtual ICollection<NotaryAvailabilityRule> NotaryAvailabilityRules { get; set; } = new List<NotaryAvailabilityRule>();

    [InverseProperty("notary")]
    public virtual ICollection<NotaryBond> NotaryBonds { get; set; } = new List<NotaryBond>();

    [InverseProperty("notary")]
    public virtual ICollection<NotaryCapability> NotaryCapabilities { get; set; } = new List<NotaryCapability>();

    [InverseProperty("notary")]
    public virtual ICollection<NotaryCommission> NotaryCommissions { get; set; } = new List<NotaryCommission>();

    [InverseProperty("notary")]
    public virtual ICollection<NotaryCommissionsPayable> NotaryCommissionsPayables { get; set; } = new List<NotaryCommissionsPayable>();

    [InverseProperty("notary")]
    public virtual ICollection<NotaryDocument> NotaryDocuments { get; set; } = new List<NotaryDocument>();

    [InverseProperty("notary")]
    public virtual ICollection<NotaryInsurance> NotaryInsurances { get; set; } = new List<NotaryInsurance>();

    [InverseProperty("notary")]
    public virtual ICollection<NotaryLicense> NotaryLicenses { get; set; } = new List<NotaryLicense>();

    [InverseProperty("notary")]
    public virtual ICollection<NotaryShiftRequest> NotaryShiftRequests { get; set; } = new List<NotaryShiftRequest>();

    [InverseProperty("notary")]
    public virtual ICollection<NotaryStatusHistory> NotaryStatusHistories { get; set; } = new List<NotaryStatusHistory>();

    [ForeignKey("region_id")]
    [InverseProperty("notaries")]
    public virtual Region? Region { get; set; }

    [InverseProperty("notary")]
    public virtual ICollection<RevenueShare> RevenueShares { get; set; } = new List<RevenueShare>();

    [InverseProperty("notary")]
    public virtual ICollection<ScheduleBlock> ScheduleBlocks { get; set; } = new List<ScheduleBlock>();

    [InverseProperty("notary")]
    public virtual ICollection<SealAccessPolicy> SealAccessPolicies { get; set; } = new List<SealAccessPolicy>();

    [InverseProperty("notary")]
    public virtual ICollection<SealUsageLog> SealUsageLogs { get; set; } = new List<SealUsageLog>();

    [InverseProperty("notary")]
    public virtual ICollection<Seal> Seals { get; set; } = new List<Seal>();

    [InverseProperty("actor_notary")]
    public virtual ICollection<SecurityAuditLog> SecurityAuditLogs { get; set; } = new List<SecurityAuditLog>();

    [InverseProperty("primary_notary")]
    public virtual ICollection<SecurityIncident> SecurityIncidents { get; set; } = new List<SecurityIncident>();

    [ForeignKey("tenant_id")]
    [InverseProperty("notaries")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("user_id")]
    [InverseProperty("notary")]
    public virtual User User { get; set; } = null!;
}

// <auto-enum-partial>
public partial class Notary
{
    public NotaryStatus status { get; set; }
}
// </auto-enum-partial>
