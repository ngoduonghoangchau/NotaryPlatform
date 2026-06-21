using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Application users and staff accounts.
/// </summary>
[Table("users", Schema = "core")]
[Index("branch_id", Name = "ix_users_branch_id")]
[Index("organization_id", Name = "ix_users_organization_id")]
[Index("team_id", Name = "ix_users_team_id")]
[Index("tenant_id", Name = "ix_users_tenant_id")]
[Index("tenant_id", "user_code", Name = "uq_users_tenant_code", IsUnique = true)]
[Index("tenant_id", "email", Name = "uq_users_tenant_email", IsUnique = true)]
public partial class User
{
    [Key]
    public Guid UserId { get; set; }

    public Guid TenantId { get; set; }

    public Guid? OrganizationId { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? TeamId { get; set; }

    [StringLength(50)]
    public string UserCode { get; set; } = null!;

    [Column(TypeName = "citext")]
    public string Email { get; set; } = null!;

    [StringLength(30)]
    public string? Phone { get; set; }

    public string PasswordHash { get; set; } = null!;

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(200)]
    public string? DisplayName { get; set; }

    [StringLength(20)]
    public string Locale { get; set; } = null!;

    [StringLength(64)]
    public string TimeZone { get; set; } = null!;

    public DateTime? LastLoginAt { get; set; }

    [Column(TypeName = "jsonb")]
    public string Settings { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("actor_user")]
    public virtual ICollection<ActAuditLog> ActAuditLogs { get; set; } = new List<ActAuditLog>();

    [InverseProperty("uploaded_by_user")]
    public virtual ICollection<ActDocument> ActDocuments { get; set; } = new List<ActDocument>();

    [InverseProperty("executed_by_user")]
    public virtual ICollection<ActExecutionRecord> ActExecutionRecords { get; set; } = new List<ActExecutionRecord>();

    [InverseProperty("verified_by_user")]
    public virtual ICollection<ActIdentityVerification> ActIdentityVerifications { get; set; } = new List<ActIdentityVerification>();

    [InverseProperty("changed_by_user")]
    public virtual ICollection<ActStatusHistory> ActStatusHistories { get; set; } = new List<ActStatusHistory>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<BillingAdjustment> BillingAdjustmentcreatedByUsers { get; set; } = new List<BillingAdjustment>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<BillingAdjustment> BillingAdjustmentupdatedByUsers { get; set; } = new List<BillingAdjustment>();

    [InverseProperty("actor_user")]
    public virtual ICollection<BillingAuditLog> BillingAuditLogs { get; set; } = new List<BillingAuditLog>();

    [ForeignKey("branch_id")]
    [InverseProperty("users")]
    public virtual Branch? Branch { get; set; }

    [InverseProperty("callee_user")]
    public virtual ICollection<CallLog> CallLogcalleeUsers { get; set; } = new List<CallLog>();

    [InverseProperty("caller_user")]
    public virtual ICollection<CallLog> CallLogcallerUsers { get; set; } = new List<CallLog>();

    [InverseProperty("uploaded_by_user")]
    public virtual ICollection<CommunicationAttachment> CommunicationAttachments { get; set; } = new List<CommunicationAttachment>();

    [InverseProperty("actor_user")]
    public virtual ICollection<CommunicationAuditLog> CommunicationAuditLogs { get; set; } = new List<CommunicationAuditLog>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<CommunicationMessage> CommunicationMessagecreatedByUsers { get; set; } = new List<CommunicationMessage>();

    [InverseProperty("sender_user")]
    public virtual ICollection<CommunicationMessage> CommunicationMessagesenderUsers { get; set; } = new List<CommunicationMessage>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<CommunicationMessage> CommunicationMessageupdatedByUsers { get; set; } = new List<CommunicationMessage>();

    [InverseProperty("user")]
    public virtual ICollection<CommunicationParticipant> CommunicationParticipants { get; set; } = new List<CommunicationParticipant>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<CommunicationReminder> CommunicationRemindercreatedByUsers { get; set; } = new List<CommunicationReminder>();

    [InverseProperty("recipient_user")]
    public virtual ICollection<CommunicationReminder> CommunicationReminderrecipientUsers { get; set; } = new List<CommunicationReminder>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<CommunicationTemplate> CommunicationTemplatecreatedByUsers { get; set; } = new List<CommunicationTemplate>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<CommunicationTemplate> CommunicationTemplateupdatedByUsers { get; set; } = new List<CommunicationTemplate>();

    [InverseProperty("assigned_user")]
    public virtual ICollection<CommunicationThread> CommunicationThreadassignedUsers { get; set; } = new List<CommunicationThread>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<CommunicationThread> CommunicationThreadcreatedByUsers { get; set; } = new List<CommunicationThread>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<CommunicationThread> CommunicationThreadupdatedByUsers { get; set; } = new List<CommunicationThread>();

    [InverseProperty("actor_user")]
    public virtual ICollection<ComplianceAuditEvent> ComplianceAuditEvents { get; set; } = new List<ComplianceAuditEvent>();

    [InverseProperty("actor_user")]
    public virtual ICollection<ComplianceAuditLog> ComplianceAuditLogs { get; set; } = new List<ComplianceAuditLog>();

    [InverseProperty("evaluated_by_user")]
    public virtual ICollection<ComplianceCheckResult> ComplianceCheckResults { get; set; } = new List<ComplianceCheckResult>();

    [InverseProperty("triggered_by_user")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<ComplianceRule> ComplianceRulecreatedByUsers { get; set; } = new List<ComplianceRule>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<ComplianceRule> ComplianceRuleupdatedByUsers { get; set; } = new List<ComplianceRule>();

    [InverseProperty("uploaded_by_user")]
    public virtual ICollection<ContractDocument> ContractDocuments { get; set; } = new List<ContractDocument>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<Contract> ContractcreatedByUsers { get; set; } = new List<Contract>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<Contract> ContractupdatedByUsers { get; set; } = new List<Contract>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<Credit> CreditcreatedByUsers { get; set; } = new List<Credit>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<Credit> CreditupdatedByUsers { get; set; } = new List<Credit>();

    [InverseProperty("uploaded_by_user")]
    public virtual ICollection<CustomerDocument> CustomerDocumentuploadedByUsers { get; set; } = new List<CustomerDocument>();

    [InverseProperty("verified_by_user")]
    public virtual ICollection<CustomerDocument> CustomerDocumentverifiedByUsers { get; set; } = new List<CustomerDocument>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<CustomerNote> CustomerNotecreatedByUsers { get; set; } = new List<CustomerNote>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<CustomerNote> CustomerNoteupdatedByUsers { get; set; } = new List<CustomerNote>();

    [InverseProperty("assigned_by_user")]
    public virtual ICollection<CustomerSegmentAssignment> CustomerSegmentAssignments { get; set; } = new List<CustomerSegmentAssignment>();

    [InverseProperty("changed_by_user")]
    public virtual ICollection<CustomerStatusHistory> CustomerStatusHistories { get; set; } = new List<CustomerStatusHistory>();

    [InverseProperty("assigned_by_user")]
    public virtual ICollection<CustomerTagAssignment> CustomerTagAssignments { get; set; } = new List<CustomerTagAssignment>();

    [InverseProperty("account_manager_user")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<DigitalCertificate> DigitalCertificatecreatedByUsers { get; set; } = new List<DigitalCertificate>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<DigitalCertificate> DigitalCertificateupdatedByUsers { get; set; } = new List<DigitalCertificate>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<DispatchRule> DispatchRulecreatedByUsers { get; set; } = new List<DispatchRule>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<DispatchRule> DispatchRuleupdatedByUsers { get; set; } = new List<DispatchRule>();

    [InverseProperty("initiated_by_user")]
    public virtual ICollection<DispatchRun> DispatchRuns { get; set; } = new List<DispatchRun>();

    [InverseProperty("locked_by_user")]
    public virtual ICollection<EmergencyLock> EmergencyLocklockedByUsers { get; set; } = new List<EmergencyLock>();

    [InverseProperty("released_by_user")]
    public virtual ICollection<EmergencyLock> EmergencyLockreleasedByUsers { get; set; } = new List<EmergencyLock>();

    [InverseProperty("assigned_to_user")]
    public virtual ICollection<IncidentAction> IncidentActionassignedToUsers { get; set; } = new List<IncidentAction>();

    [InverseProperty("performed_by_user")]
    public virtual ICollection<IncidentAction> IncidentActionperformedByUsers { get; set; } = new List<IncidentAction>();

    [InverseProperty("assigned_to_user")]
    public virtual ICollection<Incident> IncidentassignedToUsers { get; set; } = new List<Incident>();

    [InverseProperty("reported_by_user")]
    public virtual ICollection<Incident> IncidentreportedByUsers { get; set; } = new List<Incident>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<Inspection> InspectioncreatedByUsers { get; set; } = new List<Inspection>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<Inspection> InspectionupdatedByUsers { get; set; } = new List<Inspection>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<InternalNote> InternalNotecreatedByUsers { get; set; } = new List<InternalNote>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<InternalNote> InternalNoteupdatedByUsers { get; set; } = new List<InternalNote>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<Invoice> InvoicecreatedByUsers { get; set; } = new List<Invoice>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<Invoice> InvoiceupdatedByUsers { get; set; } = new List<Invoice>();

    [InverseProperty("assigned_by_user")]
    public virtual ICollection<JobAssignment> JobAssignmentassignedByUsers { get; set; } = new List<JobAssignment>();

    [InverseProperty("released_by_user")]
    public virtual ICollection<JobAssignment> JobAssignmentreleasedByUsers { get; set; } = new List<JobAssignment>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<JobReminder> JobRemindercreatedByUsers { get; set; } = new List<JobReminder>();

    [InverseProperty("recipient_user")]
    public virtual ICollection<JobReminder> JobReminderrecipientUsers { get; set; } = new List<JobReminder>();

    [InverseProperty("assigned_dispatcher_user")]
    public virtual ICollection<JobRequest> JobRequestassignedDispatcherUsers { get; set; } = new List<JobRequest>();

    [InverseProperty("cancelled_by_user")]
    public virtual ICollection<JobRequest> JobRequestcancelledByUsers { get; set; } = new List<JobRequest>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<JobRequest> JobRequestcreatedByUsers { get; set; } = new List<JobRequest>();

    [InverseProperty("changed_by_user")]
    public virtual ICollection<JobStatusHistory> JobStatusHistories { get; set; } = new List<JobStatusHistory>();

    [InverseProperty("occurred_by_user")]
    public virtual ICollection<JobTimelineEvent> JobTimelineEventoccurredByUsers { get; set; } = new List<JobTimelineEvent>();

    [InverseProperty("related_user")]
    public virtual ICollection<JobTimelineEvent> JobTimelineEventrelatedUsers { get; set; } = new List<JobTimelineEvent>();

    [InverseProperty("cancelled_by_user")]
    public virtual ICollection<Job> JobcancelledByUsers { get; set; } = new List<Job>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<Job> JobcreatedByUsers { get; set; } = new List<Job>();

    [InverseProperty("locked_by_user")]
    public virtual ICollection<Job> JoblockedByUsers { get; set; } = new List<Job>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<Job> JobupdatedByUsers { get; set; } = new List<Job>();

    [InverseProperty("actor_user")]
    public virtual ICollection<JournalAuditLog> JournalAuditLogs { get; set; } = new List<JournalAuditLog>();

    [InverseProperty("captured_by_user")]
    public virtual ICollection<JournalEntryIdDocument> JournalEntryIdDocuments { get; set; } = new List<JournalEntryIdDocument>();

    [InverseProperty("captured_by_user")]
    public virtual ICollection<JournalEntrySignature> JournalEntrySignatures { get; set; } = new List<JournalEntrySignature>();

    [InverseProperty("captured_by_user")]
    public virtual ICollection<JournalEntryThumbprint> JournalEntryThumbprints { get; set; } = new List<JournalEntryThumbprint>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<JournalEntry> JournalEntrycreatedByUsers { get; set; } = new List<JournalEntry>();

    [InverseProperty("locked_by_user")]
    public virtual ICollection<JournalEntry> JournalEntrylockedByUsers { get; set; } = new List<JournalEntry>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<JournalEntry> JournalEntryupdatedByUsers { get; set; } = new List<JournalEntry>();

    [InverseProperty("voided_by_user")]
    public virtual ICollection<JournalEntry> JournalEntryvoidedByUsers { get; set; } = new List<JournalEntry>();

    [InverseProperty("generated_by_user")]
    public virtual ICollection<JournalExport> JournalExportgeneratedByUsers { get; set; } = new List<JournalExport>();

    [InverseProperty("requested_by_user")]
    public virtual ICollection<JournalExport> JournalExportrequestedByUsers { get; set; } = new List<JournalExport>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<JournalRetentionPolicy> JournalRetentionPolicycreatedByUsers { get; set; } = new List<JournalRetentionPolicy>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<JournalRetentionPolicy> JournalRetentionPolicyupdatedByUsers { get; set; } = new List<JournalRetentionPolicy>();

    [InverseProperty("approved_by_user")]
    public virtual ICollection<JournalTransferLog> JournalTransferLogapprovedByUsers { get; set; } = new List<JournalTransferLog>();

    [InverseProperty("requested_by_user")]
    public virtual ICollection<JournalTransferLog> JournalTransferLogrequestedByUsers { get; set; } = new List<JournalTransferLog>();

    [InverseProperty("applied_by_user")]
    public virtual ICollection<LegalHold> LegalHoldappliedByUsers { get; set; } = new List<LegalHold>();

    [InverseProperty("released_by_user")]
    public virtual ICollection<LegalHold> LegalHoldreleasedByUsers { get; set; } = new List<LegalHold>();

    [InverseProperty("user")]
    public virtual MfaDevice? MfaDevice { get; set; }

    [InverseProperty("act_locked_by_user")]
    public virtual ICollection<NotarialAct> NotarialActactLockedByUsers { get; set; } = new List<NotarialAct>();

    [InverseProperty("act_voided_by_user")]
    public virtual ICollection<NotarialAct> NotarialActactVoidedByUsers { get; set; } = new List<NotarialAct>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<NotarialAct> NotarialActcreatedByUsers { get; set; } = new List<NotarialAct>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<NotarialAct> NotarialActupdatedByUsers { get; set; } = new List<NotarialAct>();

    [InverseProperty("finalized_by_user")]
    public virtual ICollection<NotarialCertificate> NotarialCertificatefinalizedByUsers { get; set; } = new List<NotarialCertificate>();

    [InverseProperty("generated_by_user")]
    public virtual ICollection<NotarialCertificate> NotarialCertificategeneratedByUsers { get; set; } = new List<NotarialCertificate>();

    [InverseProperty("locked_by_user")]
    public virtual ICollection<NotarialCertificate> NotarialCertificatelockedByUsers { get; set; } = new List<NotarialCertificate>();

    [InverseProperty("user")]
    public virtual Notary? Notary { get; set; }

    [InverseProperty("created_by_user")]
    public virtual ICollection<NotaryAuditNote> NotaryAuditNotecreatedByUsers { get; set; } = new List<NotaryAuditNote>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<NotaryAuditNote> NotaryAuditNoteupdatedByUsers { get; set; } = new List<NotaryAuditNote>();

    [InverseProperty("approved_by_user")]
    public virtual ICollection<NotaryCommissionsPayable> NotaryCommissionsPayableapprovedByUsers { get; set; } = new List<NotaryCommissionsPayable>();

    [InverseProperty("paid_by_user")]
    public virtual ICollection<NotaryCommissionsPayable> NotaryCommissionsPayablepaidByUsers { get; set; } = new List<NotaryCommissionsPayable>();

    [InverseProperty("uploaded_by_user")]
    public virtual ICollection<NotaryDocument> NotaryDocumentuploadedByUsers { get; set; } = new List<NotaryDocument>();

    [InverseProperty("verified_by_user")]
    public virtual ICollection<NotaryDocument> NotaryDocumentverifiedByUsers { get; set; } = new List<NotaryDocument>();

    [InverseProperty("verified_by_user")]
    public virtual ICollection<NotaryLicense> NotaryLicenses { get; set; } = new List<NotaryLicense>();

    [InverseProperty("approved_by_user")]
    public virtual ICollection<NotaryShiftRequest> NotaryShiftRequestapprovedByUsers { get; set; } = new List<NotaryShiftRequest>();

    [InverseProperty("requested_by_user")]
    public virtual ICollection<NotaryShiftRequest> NotaryShiftRequestrequestedByUsers { get; set; } = new List<NotaryShiftRequest>();

    [InverseProperty("performed_by_user")]
    public virtual ICollection<NotaryStatusHistory> NotaryStatusHistories { get; set; } = new List<NotaryStatusHistory>();

    [ForeignKey("organization_id")]
    [InverseProperty("users")]
    public virtual Organization? Organization { get; set; }

    [InverseProperty("created_by_user")]
    public virtual ICollection<Payment> PaymentcreatedByUsers { get; set; } = new List<Payment>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<Payment> PaymentupdatedByUsers { get; set; } = new List<Payment>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<PolicyVersion> PolicyVersioncreatedByUsers { get; set; } = new List<PolicyVersion>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<PolicyVersion> PolicyVersionupdatedByUsers { get; set; } = new List<PolicyVersion>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<PricingPlan> PricingPlans { get; set; } = new List<PricingPlan>();

    [InverseProperty("user")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<Refund> RefundcreatedByUsers { get; set; } = new List<Refund>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<Refund> RefundupdatedByUsers { get; set; } = new List<Refund>();

    [InverseProperty("generated_by_user")]
    public virtual ICollection<RegulatoryExport> RegulatoryExportgeneratedByUsers { get; set; } = new List<RegulatoryExport>();

    [InverseProperty("requested_by_user")]
    public virtual ICollection<RegulatoryExport> RegulatoryExportrequestedByUsers { get; set; } = new List<RegulatoryExport>();

    [InverseProperty("executed_by_user")]
    public virtual ICollection<RetentionJob> RetentionJobexecutedByUsers { get; set; } = new List<RetentionJob>();

    [InverseProperty("requested_by_user")]
    public virtual ICollection<RetentionJob> RetentionJobrequestedByUsers { get; set; } = new List<RetentionJob>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<RetentionPolicy> RetentionPolicycreatedByUsers { get; set; } = new List<RetentionPolicy>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<RetentionPolicy> RetentionPolicyupdatedByUsers { get; set; } = new List<RetentionPolicy>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<RevenueShare> RevenueSharecreatedByUsers { get; set; } = new List<RevenueShare>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<RevenueShare> RevenueShareupdatedByUsers { get; set; } = new List<RevenueShare>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<ScheduleBlock> ScheduleBlockcreatedByUsers { get; set; } = new List<ScheduleBlock>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<ScheduleBlock> ScheduleBlockupdatedByUsers { get; set; } = new List<ScheduleBlock>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<SealAccessPolicy> SealAccessPolicycreatedByUsers { get; set; } = new List<SealAccessPolicy>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<SealAccessPolicy> SealAccessPolicyupdatedByUsers { get; set; } = new List<SealAccessPolicy>();

    [InverseProperty("issued_by_user")]
    public virtual ICollection<SealReplacement> SealReplacements { get; set; } = new List<SealReplacement>();

    [InverseProperty("revoked_by_user")]
    public virtual ICollection<SealRevocation> SealRevocations { get; set; } = new List<SealRevocation>();

    [InverseProperty("used_by_user")]
    public virtual ICollection<SealUsageLog> SealUsageLogs { get; set; } = new List<SealUsageLog>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<Seal> SealcreatedByUsers { get; set; } = new List<Seal>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<Seal> SealupdatedByUsers { get; set; } = new List<Seal>();

    [InverseProperty("actor_user")]
    public virtual ICollection<SecurityAuditLog> SecurityAuditLogs { get; set; } = new List<SecurityAuditLog>();

    [InverseProperty("performed_by_user")]
    public virtual ICollection<SecurityIncidentAction> SecurityIncidentActions { get; set; } = new List<SecurityIncidentAction>();

    [InverseProperty("assigned_to_user")]
    public virtual ICollection<SecurityIncident> SecurityIncidentassignedToUsers { get; set; } = new List<SecurityIncident>();

    [InverseProperty("reported_by_user")]
    public virtual ICollection<SecurityIncident> SecurityIncidentreportedByUsers { get; set; } = new List<SecurityIncident>();

    [InverseProperty("created_by_user")]
    public virtual ICollection<SlaAgreement> SlaAgreementcreatedByUsers { get; set; } = new List<SlaAgreement>();

    [InverseProperty("updated_by_user")]
    public virtual ICollection<SlaAgreement> SlaAgreementupdatedByUsers { get; set; } = new List<SlaAgreement>();

    [ForeignKey("team_id")]
    [InverseProperty("users")]
    public virtual Team? Team { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("users")]
    public virtual Tenant Tenant { get; set; } = null!;

    [InverseProperty("user")]
    public virtual ICollection<TrustedDevice> TrustedDevices { get; set; } = new List<TrustedDevice>();

    [InverseProperty("assigned_by_user")]
    public virtual ICollection<UserRole> UserRoleassignedByUsers { get; set; } = new List<UserRole>();

    [InverseProperty("user")]
    public virtual ICollection<UserRole> UserRoleusers { get; set; } = new List<UserRole>();
}

// <auto-enum-partial>
public partial class User
{
    public UserStatus status { get; set; }
}
// </auto-enum-partial>
