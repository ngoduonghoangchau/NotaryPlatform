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
using BillingAdjustment = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.BillingAdjustment;
using BillingAuditLog = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.BillingAuditLog;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Tenant root record for multi-tenant isolation.
/// </summary>
[Table("tenants", Schema = "core")]
[Index("tenant_code", Name = "uq_tenants_tenant_code", IsUnique = true)]
public partial class Tenant
{
    [Key]
    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string TenantCode { get; set; } = null!;

    [StringLength(200)]
    public string TenantName { get; set; } = null!;

    [StringLength(250)]
    public string? LegalName { get; set; }

    [StringLength(2)]
    public string? PrimaryCountryCode { get; set; }

    [StringLength(64)]
    public string DefaultTimezone { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Settings { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("tenant")]
    public virtual ICollection<AccountsReceivableSnapshot> AccountsReceivableSnapshots { get; set; } = new List<AccountsReceivableSnapshot>();

    [InverseProperty("tenant")]
    public virtual ICollection<ActAuditLog> ActAuditLogs { get; set; } = new List<ActAuditLog>();

    [InverseProperty("tenant")]
    public virtual ICollection<ActDocument> ActDocuments { get; set; } = new List<ActDocument>();

    [InverseProperty("tenant")]
    public virtual ICollection<ActExecutionRecord> ActExecutionRecords { get; set; } = new List<ActExecutionRecord>();

    [InverseProperty("tenant")]
    public virtual ICollection<ActIdentityVerification> ActIdentityVerifications { get; set; } = new List<ActIdentityVerification>();

    [InverseProperty("tenant")]
    public virtual ICollection<ActSigner> ActSigners { get; set; } = new List<ActSigner>();

    [InverseProperty("tenant")]
    public virtual ICollection<ActStatusHistory> ActStatusHistories { get; set; } = new List<ActStatusHistory>();

    [InverseProperty("tenant")]
    public virtual ICollection<BillingAdjustment> BillingAdjustments { get; set; } = new List<BillingAdjustment>();

    [InverseProperty("tenant")]
    public virtual ICollection<BillingAuditLog> BillingAuditLogs { get; set; } = new List<BillingAuditLog>();

    [InverseProperty("tenant")]
    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    [InverseProperty("tenant")]
    public virtual ICollection<CallLog> CallLogs { get; set; } = new List<CallLog>();

    [InverseProperty("tenant")]
    public virtual ICollection<CommunicationAttachment> CommunicationAttachments { get; set; } = new List<CommunicationAttachment>();

    [InverseProperty("tenant")]
    public virtual ICollection<CommunicationAuditLog> CommunicationAuditLogs { get; set; } = new List<CommunicationAuditLog>();

    [InverseProperty("tenant")]
    public virtual ICollection<CommunicationDeliveryLog> CommunicationDeliveryLogs { get; set; } = new List<CommunicationDeliveryLog>();

    [InverseProperty("tenant")]
    public virtual ICollection<CommunicationMessage> CommunicationMessages { get; set; } = new List<CommunicationMessage>();

    [InverseProperty("tenant")]
    public virtual ICollection<CommunicationParticipant> CommunicationParticipants { get; set; } = new List<CommunicationParticipant>();

    [InverseProperty("tenant")]
    public virtual ICollection<CommunicationReminder> CommunicationReminders { get; set; } = new List<CommunicationReminder>();

    [InverseProperty("tenant")]
    public virtual ICollection<CommunicationTemplate> CommunicationTemplates { get; set; } = new List<CommunicationTemplate>();

    [InverseProperty("tenant")]
    public virtual ICollection<CommunicationThread> CommunicationThreads { get; set; } = new List<CommunicationThread>();

    [InverseProperty("tenant")]
    public virtual ICollection<ComplianceAuditEvent> ComplianceAuditEvents { get; set; } = new List<ComplianceAuditEvent>();

    [InverseProperty("tenant")]
    public virtual ICollection<ComplianceAuditLog> ComplianceAuditLogs { get; set; } = new List<ComplianceAuditLog>();

    [InverseProperty("tenant")]
    public virtual ICollection<ComplianceCheckResult> ComplianceCheckResults { get; set; } = new List<ComplianceCheckResult>();

    [InverseProperty("tenant")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [InverseProperty("tenant")]
    public virtual ICollection<ComplianceRule> ComplianceRules { get; set; } = new List<ComplianceRule>();

    [InverseProperty("tenant")]
    public virtual ICollection<ContractDocument> ContractDocuments { get; set; } = new List<ContractDocument>();

    [InverseProperty("tenant")]
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    [InverseProperty("tenant")]
    public virtual ICollection<CreditApplication> CreditApplications { get; set; } = new List<CreditApplication>();

    [InverseProperty("tenant")]
    public virtual ICollection<Credit> Credits { get; set; } = new List<Credit>();

    [InverseProperty("tenant")]
    public virtual ICollection<CustomerContactPreference> CustomerContactPreferences { get; set; } = new List<CustomerContactPreference>();

    [InverseProperty("tenant")]
    public virtual ICollection<CustomerContact> CustomerContacts { get; set; } = new List<CustomerContact>();

    [InverseProperty("tenant")]
    public virtual ICollection<CustomerDocument> CustomerDocuments { get; set; } = new List<CustomerDocument>();

    [InverseProperty("tenant")]
    public virtual ICollection<CustomerNote> CustomerNotes { get; set; } = new List<CustomerNote>();

    [InverseProperty("tenant")]
    public virtual ICollection<CustomerSegment> CustomerSegments { get; set; } = new List<CustomerSegment>();

    [InverseProperty("tenant")]
    public virtual ICollection<CustomerStatusHistory> CustomerStatusHistories { get; set; } = new List<CustomerStatusHistory>();

    [InverseProperty("tenant")]
    public virtual ICollection<CustomerTag> CustomerTags { get; set; } = new List<CustomerTag>();

    [InverseProperty("tenant")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    [InverseProperty("tenant")]
    public virtual ICollection<DailyOperationalSnapshot> DailyOperationalSnapshots { get; set; } = new List<DailyOperationalSnapshot>();

    [InverseProperty("tenant")]
    public virtual ICollection<DigitalCertificateChainItem> DigitalCertificateChainItems { get; set; } = new List<DigitalCertificateChainItem>();

    [InverseProperty("tenant")]
    public virtual ICollection<DigitalCertificate> DigitalCertificates { get; set; } = new List<DigitalCertificate>();

    [InverseProperty("tenant")]
    public virtual ICollection<DispatchCandidate> DispatchCandidates { get; set; } = new List<DispatchCandidate>();

    [InverseProperty("tenant")]
    public virtual ICollection<DispatchRule> DispatchRules { get; set; } = new List<DispatchRule>();

    [InverseProperty("tenant")]
    public virtual ICollection<DispatchRun> DispatchRuns { get; set; } = new List<DispatchRun>();

    [InverseProperty("tenant")]
    public virtual ICollection<EmergencyLock> EmergencyLocks { get; set; } = new List<EmergencyLock>();

    [InverseProperty("tenant")]
    public virtual ICollection<IncidentAction> IncidentActions { get; set; } = new List<IncidentAction>();

    [InverseProperty("tenant")]
    public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    [InverseProperty("tenant")]
    public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();

    [InverseProperty("tenant")]
    public virtual ICollection<InternalNote> InternalNotes { get; set; } = new List<InternalNote>();

    [InverseProperty("tenant")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    [InverseProperty("tenant")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("tenant")]
    public virtual ICollection<JobAssignment> JobAssignments { get; set; } = new List<JobAssignment>();

    [InverseProperty("tenant")]
    public virtual ICollection<JobReminder> JobReminders { get; set; } = new List<JobReminder>();

    [InverseProperty("tenant")]
    public virtual ICollection<JobRequest> JobRequests { get; set; } = new List<JobRequest>();

    [InverseProperty("tenant")]
    public virtual ICollection<JobStatusHistory> JobStatusHistories { get; set; } = new List<JobStatusHistory>();

    [InverseProperty("tenant")]
    public virtual ICollection<JobTimelineEvent> JobTimelineEvents { get; set; } = new List<JobTimelineEvent>();

    [InverseProperty("tenant")]
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    [InverseProperty("tenant")]
    public virtual ICollection<JournalAuditLog> JournalAuditLogs { get; set; } = new List<JournalAuditLog>();

    [InverseProperty("tenant")]
    public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();

    [InverseProperty("tenant")]
    public virtual ICollection<JournalEntryIdDocument> JournalEntryIdDocuments { get; set; } = new List<JournalEntryIdDocument>();

    [InverseProperty("tenant")]
    public virtual ICollection<JournalEntryLink> JournalEntryLinks { get; set; } = new List<JournalEntryLink>();

    [InverseProperty("tenant")]
    public virtual ICollection<JournalEntrySignature> JournalEntrySignatures { get; set; } = new List<JournalEntrySignature>();

    [InverseProperty("tenant")]
    public virtual ICollection<JournalEntrySigner> JournalEntrySigners { get; set; } = new List<JournalEntrySigner>();

    [InverseProperty("tenant")]
    public virtual ICollection<JournalEntryThumbprint> JournalEntryThumbprints { get; set; } = new List<JournalEntryThumbprint>();

    [InverseProperty("tenant")]
    public virtual ICollection<JournalExport> JournalExports { get; set; } = new List<JournalExport>();

    [InverseProperty("tenant")]
    public virtual ICollection<JournalRetentionPolicy> JournalRetentionPolicies { get; set; } = new List<JournalRetentionPolicy>();

    [InverseProperty("tenant")]
    public virtual ICollection<JournalTransferLog> JournalTransferLogs { get; set; } = new List<JournalTransferLog>();

    [InverseProperty("tenant")]
    public virtual ICollection<LegalHold> LegalHolds { get; set; } = new List<LegalHold>();

    [InverseProperty("tenant")]
    public virtual ICollection<MfaDevice> MfaDevices { get; set; } = new List<MfaDevice>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotarialAct> NotarialActs { get; set; } = new List<NotarialAct>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotarialCertificate> NotarialCertificates { get; set; } = new List<NotarialCertificate>();

    [InverseProperty("tenant")]
    public virtual ICollection<Notary> Notaries { get; set; } = new List<Notary>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotaryAuditNote> NotaryAuditNotes { get; set; } = new List<NotaryAuditNote>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotaryAvailabilityRule> NotaryAvailabilityRules { get; set; } = new List<NotaryAvailabilityRule>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotaryBond> NotaryBonds { get; set; } = new List<NotaryBond>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotaryCapability> NotaryCapabilities { get; set; } = new List<NotaryCapability>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotaryCommission> NotaryCommissions { get; set; } = new List<NotaryCommission>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotaryCommissionsPayable> NotaryCommissionsPayables { get; set; } = new List<NotaryCommissionsPayable>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotaryDocument> NotaryDocuments { get; set; } = new List<NotaryDocument>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotaryInsurance> NotaryInsurances { get; set; } = new List<NotaryInsurance>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotaryLicense> NotaryLicenses { get; set; } = new List<NotaryLicense>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotaryShiftRequest> NotaryShiftRequests { get; set; } = new List<NotaryShiftRequest>();

    [InverseProperty("tenant")]
    public virtual ICollection<NotaryStatusHistory> NotaryStatusHistories { get; set; } = new List<NotaryStatusHistory>();

    [InverseProperty("tenant")]
    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    [InverseProperty("tenant")]
    public virtual ICollection<PaymentAllocation> PaymentAllocations { get; set; } = new List<PaymentAllocation>();

    [InverseProperty("tenant")]
    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();

    [InverseProperty("tenant")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("tenant")]
    public virtual ICollection<PolicyVersion> PolicyVersions { get; set; } = new List<PolicyVersion>();

    [InverseProperty("tenant")]
    public virtual ICollection<PricingPlan> PricingPlans { get; set; } = new List<PricingPlan>();

    [InverseProperty("tenant")]
    public virtual ICollection<PricingRule> PricingRules { get; set; } = new List<PricingRule>();

    [InverseProperty("tenant")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [InverseProperty("tenant")]
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    [InverseProperty("tenant")]
    public virtual ICollection<Region> Regions { get; set; } = new List<Region>();

    [InverseProperty("tenant")]
    public virtual ICollection<RegulatoryExport> RegulatoryExports { get; set; } = new List<RegulatoryExport>();

    [InverseProperty("tenant")]
    public virtual ICollection<RetentionJob> RetentionJobs { get; set; } = new List<RetentionJob>();

    [InverseProperty("tenant")]
    public virtual ICollection<RetentionPolicy> RetentionPolicies { get; set; } = new List<RetentionPolicy>();

    [InverseProperty("tenant")]
    public virtual ICollection<RevenueShare> RevenueShares { get; set; } = new List<RevenueShare>();

    [InverseProperty("tenant")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    [InverseProperty("tenant")]
    public virtual ICollection<ScheduleBlock> ScheduleBlocks { get; set; } = new List<ScheduleBlock>();

    [InverseProperty("tenant")]
    public virtual ICollection<SealAccessPolicy> SealAccessPolicies { get; set; } = new List<SealAccessPolicy>();

    [InverseProperty("tenant")]
    public virtual ICollection<SealReplacement> SealReplacements { get; set; } = new List<SealReplacement>();

    [InverseProperty("tenant")]
    public virtual ICollection<SealRevocation> SealRevocations { get; set; } = new List<SealRevocation>();

    [InverseProperty("tenant")]
    public virtual ICollection<SealUsageLog> SealUsageLogs { get; set; } = new List<SealUsageLog>();

    [InverseProperty("tenant")]
    public virtual ICollection<Seal> Seals { get; set; } = new List<Seal>();

    [InverseProperty("tenant")]
    public virtual ICollection<SecurityAuditLog> SecurityAuditLogs { get; set; } = new List<SecurityAuditLog>();

    [InverseProperty("tenant")]
    public virtual ICollection<SecurityIncidentAction> SecurityIncidentActions { get; set; } = new List<SecurityIncidentAction>();

    [InverseProperty("tenant")]
    public virtual ICollection<SecurityIncident> SecurityIncidents { get; set; } = new List<SecurityIncident>();

    [InverseProperty("tenant")]
    public virtual ICollection<ServiceType> ServiceTypes { get; set; } = new List<ServiceType>();

    [InverseProperty("tenant")]
    public virtual ICollection<SlaAgreement> SlaAgreements { get; set; } = new List<SlaAgreement>();

    [InverseProperty("tenant")]
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    [InverseProperty("tenant")]
    public virtual ICollection<TrustedDevice> TrustedDevices { get; set; } = new List<TrustedDevice>();

    [InverseProperty("tenant")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    [InverseProperty("tenant")]
    public virtual ICollection<VoidReason> VoidReasons { get; set; } = new List<VoidReason>();
}

// <auto-enum-partial>
public partial class Tenant
{
    public TenantStatus status { get; set; }
}
// </auto-enum-partial>
