using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;
using NotaryPlatform.Infrastructure.Persistence.ReadModels;

namespace NotaryPlatform.Infrastructure.Persistence.DbContexts;

public partial class NotaryPlatformDbContext : DbContext
{
    public NotaryPlatformDbContext(DbContextOptions<NotaryPlatformDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountsReceivableSnapshot> AccountsReceivableSnapshots { get; set; }

    public virtual DbSet<ActAuditLog> ActAuditLogs { get; set; }

    public virtual DbSet<ActDocument> ActDocuments { get; set; }

    public virtual DbSet<ActExecutionRecord> ActExecutionRecords { get; set; }

    public virtual DbSet<ActIdentityVerification> ActIdentityVerifications { get; set; }

    public virtual DbSet<ActSigner> ActSigners { get; set; }

    public virtual DbSet<ActStatusHistory> ActStatusHistories { get; set; }

    public virtual DbSet<BillingAdjustment> BillingAdjustments { get; set; }

    public virtual DbSet<BillingAuditLog> BillingAuditLogs { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<CallLog> CallLogs { get; set; }

    public virtual DbSet<CommunicationAttachment> CommunicationAttachments { get; set; }

    public virtual DbSet<CommunicationAuditLog> CommunicationAuditLogs { get; set; }

    public virtual DbSet<CommunicationDeliveryLog> CommunicationDeliveryLogs { get; set; }

    public virtual DbSet<CommunicationMessage> CommunicationMessages { get; set; }

    public virtual DbSet<CommunicationParticipant> CommunicationParticipants { get; set; }

    public virtual DbSet<CommunicationReminder> CommunicationReminders { get; set; }

    public virtual DbSet<CommunicationTemplate> CommunicationTemplates { get; set; }

    public virtual DbSet<CommunicationThread> CommunicationThreads { get; set; }

    public virtual DbSet<ComplianceAuditEvent> ComplianceAuditEvents { get; set; }

    public virtual DbSet<ComplianceAuditLog> ComplianceAuditLogs { get; set; }

    public virtual DbSet<ComplianceCheck> ComplianceChecks { get; set; }

    public virtual DbSet<ComplianceCheckResult> ComplianceCheckResults { get; set; }

    public virtual DbSet<ComplianceRule> ComplianceRules { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<ContractDocument> ContractDocuments { get; set; }

    public virtual DbSet<Credit> Credits { get; set; }

    public virtual DbSet<CreditApplication> CreditApplications { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerContact> CustomerContacts { get; set; }

    public virtual DbSet<CustomerContactPreference> CustomerContactPreferences { get; set; }

    public virtual DbSet<CustomerDocument> CustomerDocuments { get; set; }

    public virtual DbSet<CustomerNote> CustomerNotes { get; set; }

    public virtual DbSet<CustomerSegment> CustomerSegments { get; set; }

    public virtual DbSet<CustomerSegmentAssignment> CustomerSegmentAssignments { get; set; }

    public virtual DbSet<CustomerStatusHistory> CustomerStatusHistories { get; set; }

    public virtual DbSet<CustomerTag> CustomerTags { get; set; }

    public virtual DbSet<CustomerTagAssignment> CustomerTagAssignments { get; set; }

    public virtual DbSet<DailyOperationalSnapshot> DailyOperationalSnapshots { get; set; }

    public virtual DbSet<DigitalCertificate> DigitalCertificates { get; set; }

    public virtual DbSet<DigitalCertificateChainItem> DigitalCertificateChainItems { get; set; }

    public virtual DbSet<DispatchCandidate> DispatchCandidates { get; set; }

    public virtual DbSet<DispatchRule> DispatchRules { get; set; }

    public virtual DbSet<DispatchRun> DispatchRuns { get; set; }

    public virtual DbSet<EmergencyLock> EmergencyLocks { get; set; }

    public virtual DbSet<Incident> Incidents { get; set; }

    public virtual DbSet<IncidentAction> IncidentActions { get; set; }

    public virtual DbSet<Inspection> Inspections { get; set; }

    public virtual DbSet<InternalNote> InternalNotes { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobAssignment> JobAssignments { get; set; }

    public virtual DbSet<JobReminder> JobReminders { get; set; }

    public virtual DbSet<JobRequest> JobRequests { get; set; }

    public virtual DbSet<JobStatusHistory> JobStatusHistories { get; set; }

    public virtual DbSet<JobTimelineEvent> JobTimelineEvents { get; set; }

    public virtual DbSet<JournalAuditLog> JournalAuditLogs { get; set; }

    public virtual DbSet<JournalEntry> JournalEntries { get; set; }

    public virtual DbSet<JournalEntryIdDocument> JournalEntryIdDocuments { get; set; }

    public virtual DbSet<JournalEntryLink> JournalEntryLinks { get; set; }

    public virtual DbSet<JournalEntrySignature> JournalEntrySignatures { get; set; }

    public virtual DbSet<JournalEntrySigner> JournalEntrySigners { get; set; }

    public virtual DbSet<JournalEntryThumbprint> JournalEntryThumbprints { get; set; }

    public virtual DbSet<JournalExport> JournalExports { get; set; }

    public virtual DbSet<JournalRetentionPolicy> JournalRetentionPolicies { get; set; }

    public virtual DbSet<JournalTransferLog> JournalTransferLogs { get; set; }

    public virtual DbSet<LegalHold> LegalHolds { get; set; }

    public virtual DbSet<MfaDevice> MfaDevices { get; set; }

    public virtual DbSet<NotarialAct> NotarialActs { get; set; }

    public virtual DbSet<NotarialCertificate> NotarialCertificates { get; set; }

    public virtual DbSet<Notary> Notaries { get; set; }

    public virtual DbSet<NotaryAuditNote> NotaryAuditNotes { get; set; }

    public virtual DbSet<NotaryAvailabilityRule> NotaryAvailabilityRules { get; set; }

    public virtual DbSet<NotaryBond> NotaryBonds { get; set; }

    public virtual DbSet<NotaryCapability> NotaryCapabilities { get; set; }

    public virtual DbSet<NotaryCommission> NotaryCommissions { get; set; }

    public virtual DbSet<NotaryCommissionsPayable> NotaryCommissionsPayables { get; set; }

    public virtual DbSet<NotaryDocument> NotaryDocuments { get; set; }

    public virtual DbSet<NotaryInsurance> NotaryInsurances { get; set; }

    public virtual DbSet<NotaryLicense> NotaryLicenses { get; set; }

    public virtual DbSet<NotaryShiftRequest> NotaryShiftRequests { get; set; }

    public virtual DbSet<NotaryStatusHistory> NotaryStatusHistories { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentAllocation> PaymentAllocations { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PolicyVersion> PolicyVersions { get; set; }

    public virtual DbSet<PricingPlan> PricingPlans { get; set; }

    public virtual DbSet<PricingRule> PricingRules { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<RegulatoryExport> RegulatoryExports { get; set; }

    public virtual DbSet<RetentionJob> RetentionJobs { get; set; }

    public virtual DbSet<RetentionPolicy> RetentionPolicies { get; set; }

    public virtual DbSet<RevenueShare> RevenueShares { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<ScheduleBlock> ScheduleBlocks { get; set; }

    public virtual DbSet<Seal> Seals { get; set; }

    public virtual DbSet<SealAccessPolicy> SealAccessPolicies { get; set; }

    public virtual DbSet<SealReplacement> SealReplacements { get; set; }

    public virtual DbSet<SealRevocation> SealRevocations { get; set; }

    public virtual DbSet<SealUsageLog> SealUsageLogs { get; set; }

    public virtual DbSet<SecurityAuditLog> SecurityAuditLogs { get; set; }

    public virtual DbSet<SecurityIncident> SecurityIncidents { get; set; }

    public virtual DbSet<SecurityIncidentAction> SecurityIncidentActions { get; set; }

    public virtual DbSet<ServiceType> ServiceTypes { get; set; }

    public virtual DbSet<SlaAgreement> SlaAgreements { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<TrustedDevice> TrustedDevices { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<VActLatestCertificate> VActLatestCertificates { get; set; }

    public virtual DbSet<VActOverview> VActOverviews { get; set; }

    public virtual DbSet<VActiveRule> VActiveRules { get; set; }

    public virtual DbSet<VActiveSeal> VActiveSeals { get; set; }

    public virtual DbSet<VActiveUser> VActiveUsers { get; set; }

    public virtual DbSet<VAgingSummary> VAgingSummaries { get; set; }

    public virtual DbSet<VCertificateInventory> VCertificateInventories { get; set; }

    public virtual DbSet<VCurrentPolicyVersion> VCurrentPolicyVersions { get; set; }

    public virtual DbSet<VCurrentPrimaryAssignment> VCurrentPrimaryAssignments { get; set; }

    public virtual DbSet<VCustomerOverview> VCustomerOverviews { get; set; }

    public virtual DbSet<VCustomerPrimaryContact> VCustomerPrimaryContacts { get; set; }

    public virtual DbSet<VInvoiceOverview> VInvoiceOverviews { get; set; }

    public virtual DbSet<VJobBoard> VJobBoards { get; set; }

    public virtual DbSet<VJournalComplianceSummary> VJournalComplianceSummaries { get; set; }

    public virtual DbSet<VJournalEntryOverview> VJournalEntryOverviews { get; set; }

    public virtual DbSet<VJournalEntrySignerOverview> VJournalEntrySignerOverviews { get; set; }

    public virtual DbSet<VLatestMessagePerThread> VLatestMessagePerThreads { get; set; }

    public virtual DbSet<VNotaryComplianceStatus> VNotaryComplianceStatuses { get; set; }

    public virtual DbSet<VNotaryDailySchedule> VNotaryDailySchedules { get; set; }

    public virtual DbSet<VNotaryProfileOverview> VNotaryProfileOverviews { get; set; }

    public virtual DbSet<VOpenIncident> VOpenIncidents { get; set; }

    public virtual DbSet<VPendingReminder> VPendingReminders { get; set; }

    public virtual DbSet<VRevenueShareSummary> VRevenueShareSummaries { get; set; }

    public virtual DbSet<VSecurityRiskSummary> VSecurityRiskSummaries { get; set; }

    public virtual DbSet<VThreadOverview> VThreadOverviews { get; set; }

    public virtual DbSet<VoidReason> VoidReasons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("billing", "adjustment_status", new[] { "draft", "posted", "reversed", "voided" })
            .HasPostgresEnum("billing", "aging_bucket", new[] { "current", "days_1_30", "days_31_60", "days_61_90", "days_90_plus" })
            .HasPostgresEnum("billing", "audit_event_type", new[] { "create", "update", "issue_invoice", "send_invoice", "record_payment", "apply_credit", "apply_refund", "post_adjustment", "void_invoice", "write_off", "accrue_payable", "pay_payable", "status_change", "export", "import" })
            .HasPostgresEnum("billing", "credit_status", new[] { "available", "partially_used", "used", "expired", "voided" })
            .HasPostgresEnum("billing", "invoice_item_type", new[] { "service_fee", "travel_fee", "ron_fee", "printing_fee", "late_fee", "discount", "adjustment", "tax", "other" })
            .HasPostgresEnum("billing", "invoice_status", new[] { "draft", "issued", "partially_paid", "paid", "overdue", "voided", "cancelled", "written_off" })
            .HasPostgresEnum("billing", "payable_status", new[] { "pending", "accrued", "approved", "paid", "partially_paid", "cancelled", "reversed" })
            .HasPostgresEnum("billing", "payment_method_type", new[] { "credit_card", "ach", "cash", "check", "bank_transfer", "wallet", "other" })
            .HasPostgresEnum("billing", "payment_status", new[] { "pending", "authorized", "captured", "settled", "failed", "reversed", "refunded", "voided" })
            .HasPostgresEnum("billing", "refund_status", new[] { "pending", "approved", "processed", "failed", "cancelled" })
            .HasPostgresEnum("billing", "revenue_share_type", new[] { "percentage", "fixed", "hybrid", "bonus", "deduction" })
            .HasPostgresEnum("communication", "audit_event_type", new[] { "create", "update", "send", "deliver", "read", "fail", "cancel", "open_thread", "close_thread", "archive_thread", "add_participant", "remove_participant", "attach_file", "remove_file", "create_note", "update_note", "schedule_reminder", "complete_reminder", "schedule_call", "complete_call" })
            .HasPostgresEnum("communication", "call_outcome", new[] { "connected", "no_answer", "voicemail_left", "callback_requested", "follow_up_needed", "resolved", "escalated", "other" })
            .HasPostgresEnum("communication", "call_status", new[] { "scheduled", "in_progress", "completed", "no_answer", "voicemail", "cancelled", "failed" })
            .HasPostgresEnum("communication", "channel_type", new[] { "email", "sms", "phone", "meeting", "in_app", "whatsapp", "other" })
            .HasPostgresEnum("communication", "delivery_status", new[] { "pending", "queued", "sent", "delivered", "failed", "bounced", "suppressed", "cancelled" })
            .HasPostgresEnum("communication", "message_direction", new[] { "inbound", "outbound", "internal" })
            .HasPostgresEnum("communication", "message_status", new[] { "draft", "queued", "sent", "delivered", "read", "failed", "cancelled" })
            .HasPostgresEnum("communication", "note_visibility", new[] { "private", "team", "company" })
            .HasPostgresEnum("communication", "participant_role", new[] { "customer", "customer_contact", "user", "notary", "external", "observer" })
            .HasPostgresEnum("communication", "reminder_status", new[] { "pending", "queued", "sent", "delivered", "failed", "cancelled", "snoozed" })
            .HasPostgresEnum("communication", "template_type", new[] { "email", "sms", "in_app", "call_script", "meeting_agenda", "other" })
            .HasPostgresEnum("communication", "thread_status", new[] { "open", "pending", "closed", "archived" })
            .HasPostgresEnum("compliance", "audit_event_type", new[] { "create", "update", "delete", "status_change", "rule_evaluated", "check_started", "check_completed", "check_failed", "blocked", "unblocked", "hold_applied", "hold_released", "policy_versioned", "retention_applied", "retention_scheduled", "export_requested", "export_generated", "inspection_started", "inspection_completed", "incident_opened", "incident_updated", "incident_closed" })
            .HasPostgresEnum("compliance", "check_result_severity", new[] { "info", "warning", "error", "critical" })
            .HasPostgresEnum("compliance", "check_status", new[] { "pending", "running", "passed", "failed", "warning", "blocked", "skipped", "manual_review" })
            .HasPostgresEnum("compliance", "export_format", new[] { "pdf", "csv", "xlsx", "json", "zip" })
            .HasPostgresEnum("compliance", "export_status", new[] { "queued", "processing", "generated", "failed", "downloaded", "expired" })
            .HasPostgresEnum("compliance", "incident_severity", new[] { "low", "medium", "high", "critical" })
            .HasPostgresEnum("compliance", "incident_status", new[] { "open", "investigating", "contained", "resolved", "closed", "escalated" })
            .HasPostgresEnum("compliance", "incident_type", new[] { "policy_violation", "state_rule_violation", "unauthorized_access", "missing_journal_entry", "expired_seal_usage", "expired_certificate_usage", "late_filing", "data_retention_violation", "privacy_violation", "other" })
            .HasPostgresEnum("compliance", "inspection_status", new[] { "planned", "in_progress", "awaiting_response", "completed", "closed", "cancelled" })
            .HasPostgresEnum("compliance", "legal_hold_status", new[] { "active", "released", "expired", "cancelled" })
            .HasPostgresEnum("compliance", "policy_status", new[] { "draft", "active", "inactive", "archived" })
            .HasPostgresEnum("compliance", "rule_scope", new[] { "tenant", "branch", "region", "state", "notary", "role", "customer", "service_type", "job", "act", "journal_entry", "seal", "certificate" })
            .HasPostgresEnum("compliance", "rule_severity", new[] { "low", "medium", "high", "critical" })
            .HasPostgresEnum("compliance", "rule_status", new[] { "draft", "active", "inactive", "archived" })
            .HasPostgresEnum("core", "branch_status", new[] { "active", "inactive", "suspended" })
            .HasPostgresEnum("core", "organization_status", new[] { "active", "inactive", "suspended" })
            .HasPostgresEnum("core", "organization_type", new[] { "company", "branch", "department", "team", "service_center" })
            .HasPostgresEnum("core", "region_status", new[] { "active", "inactive" })
            .HasPostgresEnum("core", "team_status", new[] { "active", "inactive" })
            .HasPostgresEnum("core", "tenant_status", new[] { "active", "suspended", "closed" })
            .HasPostgresEnum("core", "user_status", new[] { "invited", "active", "inactive", "locked", "archived" })
            .HasPostgresEnum("crm", "contact_role", new[] { "primary", "billing", "ordering", "legal", "technical", "other" })
            .HasPostgresEnum("crm", "contact_status", new[] { "active", "inactive", "archived" })
            .HasPostgresEnum("crm", "contract_status", new[] { "draft", "active", "expired", "terminated", "renewed", "cancelled" })
            .HasPostgresEnum("crm", "customer_status", new[] { "active", "inactive", "suspended", "archived" })
            .HasPostgresEnum("crm", "customer_type", new[] { "individual", "company" })
            .HasPostgresEnum("crm", "document_status", new[] { "uploaded", "verified", "rejected", "archived" })
            .HasPostgresEnum("crm", "note_visibility", new[] { "private", "team", "company" })
            .HasPostgresEnum("crm", "pricing_model", new[] { "fixed", "volume_based", "tiered", "per_job", "custom" })
            .HasPostgresEnum("crm", "pricing_rule_type", new[] { "base_rate", "volume_tier", "discount", "surcharge", "custom_exception" })
            .HasPostgresEnum("crm", "segment_type", new[] { "vip", "high_volume", "preferred", "risk", "custom" })
            .HasPostgresEnum("crm", "sla_status", new[] { "draft", "active", "expired", "breached", "terminated" })
            .HasPostgresEnum("identity", "bond_status", new[] { "valid", "expiring", "expired", "missing", "cancelled" })
            .HasPostgresEnum("identity", "capability_code", new[] { "acknowledgment", "jurat", "copy_certification", "mobile_notary", "ron", "loan_signing", "apostille_support" })
            .HasPostgresEnum("identity", "commission_status", new[] { "pending", "active", "expiring", "expired", "revoked", "suspended" })
            .HasPostgresEnum("identity", "document_status", new[] { "uploaded", "verified", "rejected", "expired", "archived" })
            .HasPostgresEnum("identity", "history_action_type", new[] { "create", "update", "activate", "suspend", "reactivate", "expire", "revoke", "block", "unlock", "upload_document", "verify_document", "reject_document" })
            .HasPostgresEnum("identity", "insurance_status", new[] { "valid", "expiring", "expired", "missing", "cancelled" })
            .HasPostgresEnum("identity", "notary_status", new[] { "active", "inactive", "suspended", "blocked", "expired", "pending" })
            .HasPostgresEnum("journal", "journal_audit_event_type", new[] { "create", "update", "status_change", "lock", "unlock", "void", "export", "transfer", "import", "attachment_added", "attachment_removed", "field_changed", "retention_applied" })
            .HasPostgresEnum("journal", "journal_capture_type", new[] { "signature", "thumbprint", "both" })
            .HasPostgresEnum("journal", "journal_entry_status", new[] { "draft", "pending", "completed", "locked", "voided", "archived" })
            .HasPostgresEnum("journal", "journal_export_format", new[] { "pdf", "csv", "xlsx", "json" })
            .HasPostgresEnum("journal", "journal_export_status", new[] { "queued", "generated", "failed", "downloaded", "expired" })
            .HasPostgresEnum("journal", "journal_field_source", new[] { "auto_populated", "manual", "imported", "system_generated" })
            .HasPostgresEnum("journal", "journal_transfer_type", new[] { "internal_custodian", "regulator", "successor_notary", "archive", "legal_hold" })
            .HasPostgresEnum("journal", "journal_verification_method", new[] { "physical_presence", "remote_online_notarization", "kba", "id_scan", "credential_analysis", "other" })
            .HasPostgresEnum("notarial", "act_event_type", new[] { "create", "update", "status_change", "verification_added", "execution_started", "execution_completed", "certificate_generated", "certificate_finalized", "journal_linked", "locked", "voided", "unlocked", "document_attached", "document_removed" })
            .HasPostgresEnum("notarial", "appearance_type", new[] { "physical", "remote", "hybrid" })
            .HasPostgresEnum("notarial", "certificate_status", new[] { "draft", "generated", "previewed", "finalized", "locked", "voided" })
            .HasPostgresEnum("notarial", "document_link_type", new[] { "subject_document", "supporting_document", "id_document", "attachment", "evidence", "other" })
            .HasPostgresEnum("notarial", "execution_status", new[] { "not_started", "in_progress", "completed", "abandoned", "requires_review" })
            .HasPostgresEnum("notarial", "identity_verification_method", new[] { "physical_presence", "government_id", "kba", "credential_analysis", "biometric", "video_conference", "remote_online_notarization", "other" })
            .HasPostgresEnum("notarial", "notarial_act_status", new[] { "draft", "pending_verification", "in_execution", "awaiting_certificate", "awaiting_journal", "completed", "locked", "voided", "cancelled", "archived" })
            .HasPostgresEnum("notarial", "notarial_act_type", new[] { "acknowledgment", "jurat", "oath_affirmation", "copy_certification", "signature_witnessing", "loan_signing", "affidavit", "power_of_attorney", "ron_acknowledgment", "ron_jurat", "other" })
            .HasPostgresEnum("notarial", "signer_role", new[] { "signer", "principal", "witness", "agent", "attorney_in_fact", "translator", "other" })
            .HasPostgresEnum("notarial", "verification_result", new[] { "pending", "passed", "failed", "expired", "incomplete", "manual_review" })
            .HasPostgresEnum("operations", "assignment_role", new[] { "primary", "backup", "observer", "substitute" })
            .HasPostgresEnum("operations", "assignment_status", new[] { "proposed", "assigned", "accepted", "declined", "released", "replaced", "completed" })
            .HasPostgresEnum("operations", "availability_rule_type", new[] { "working_hours", "preferred_hours", "on_call", "blackout", "unavailable" })
            .HasPostgresEnum("operations", "dispatch_candidate_status", new[] { "eligible", "preferred", "warning", "ineligible", "rejected" })
            .HasPostgresEnum("operations", "dispatch_rule_type", new[] { "distance", "service_type", "compliance", "workload", "priority", "state_restriction", "manual_override" })
            .HasPostgresEnum("operations", "dispatch_run_status", new[] { "queued", "running", "completed", "failed", "cancelled" })
            .HasPostgresEnum("operations", "job_priority", new[] { "low", "normal", "high", "urgent", "rush" })
            .HasPostgresEnum("operations", "job_request_status", new[] { "new", "triaged", "quoted", "scheduled", "converted", "rejected", "cancelled", "closed" })
            .HasPostgresEnum("operations", "job_status", new[] { "draft", "scheduled", "confirmed", "in_progress", "completed", "locked", "cancelled", "failed", "archived" })
            .HasPostgresEnum("operations", "notification_scope", new[] { "internal", "customer", "notary", "mixed" })
            .HasPostgresEnum("operations", "reminder_channel", new[] { "email", "sms", "in_app" })
            .HasPostgresEnum("operations", "reminder_recipient_type", new[] { "contact", "user", "notary" })
            .HasPostgresEnum("operations", "reminder_status", new[] { "pending", "queued", "sent", "delivered", "failed", "cancelled" })
            .HasPostgresEnum("operations", "schedule_block_type", new[] { "job", "shift", "break", "travel", "hold", "meeting", "blackout" })
            .HasPostgresEnum("operations", "service_mode", new[] { "onsite", "mobile", "ron", "hybrid" })
            .HasPostgresEnum("operations", "shift_request_status", new[] { "pending", "approved", "rejected", "cancelled", "completed" })
            .HasPostgresEnum("operations", "timeline_event_type", new[] { "status_change", "assignment_change", "note", "reminder", "reschedule", "location_change", "compliance_flag", "escalation", "attachment_added" })
            .HasPostgresEnum("security", "audit_event_type", new[] { "create", "update", "status_change", "activate", "suspend", "revoke", "replace", "lock", "unlock", "usage_allowed", "usage_denied", "incident_opened", "incident_updated", "incident_closed", "policy_changed", "device_trusted", "device_revoked", "mfa_added", "mfa_removed" })
            .HasPostgresEnum("security", "certificate_status", new[] { "pending", "active", "expiring", "expired", "suspended", "revoked", "replaced", "archived" })
            .HasPostgresEnum("security", "device_status", new[] { "pending", "trusted", "revoked", "expired" })
            .HasPostgresEnum("security", "incident_severity", new[] { "low", "medium", "high", "critical" })
            .HasPostgresEnum("security", "incident_status", new[] { "open", "investigating", "contained", "resolved", "closed", "escalated" })
            .HasPostgresEnum("security", "incident_type", new[] { "lost_seal", "stolen_seal", "compromised_private_key", "unauthorized_use", "expired_asset_use", "suspicious_activity", "access_violation", "data_breach", "other" })
            .HasPostgresEnum("security", "lock_status", new[] { "active", "released", "expired", "cancelled" })
            .HasPostgresEnum("security", "mfa_method_type", new[] { "totp", "sms", "email", "push", "hardware_key", "recovery_code" })
            .HasPostgresEnum("security", "policy_scope", new[] { "tenant", "branch", "region", "state", "notary", "role" })
            .HasPostgresEnum("security", "policy_status", new[] { "draft", "active", "inactive", "archived" })
            .HasPostgresEnum("security", "policy_target_type", new[] { "seal", "certificate", "both" })
            .HasPostgresEnum("security", "replacement_reason_type", new[] { "lost", "stolen", "compromised", "expired", "damaged", "renewal", "other" })
            .HasPostgresEnum("security", "seal_status", new[] { "active", "suspended", "revoked", "expired", "lost", "stolen", "replaced", "archived" })
            .HasPostgresEnum("security", "seal_type", new[] { "physical_seal", "electronic_seal", "embosser", "stamp" })
            .HasPostgresEnum("security", "usage_action_type", new[] { "apply_seal", "apply_certificate", "verify_signature", "generate_seal_artifact", "sign_document", "validate_usage" })
            .HasPostgresEnum("security", "usage_result_type", new[] { "allowed", "denied", "pending_approval", "failed" })
            .HasPostgresExtension("citext")
            .HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<AccountsReceivableSnapshot>(entity =>
        {
            entity.HasKey(e => e.ArSnapshotId).HasName("accounts_receivable_snapshots_pkey");

            entity.ToTable("accounts_receivable_snapshots", "billing", tb => tb.HasComment("AR aging snapshots for dashboards and reporting."));

            entity.Property(e => e.ArSnapshotId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.InvoiceCount).HasDefaultValue(0);
            entity.Property(e => e.Metrics).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OverdueInvoiceCount).HasDefaultValue(0);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Branch).WithMany(p => p.AccountsReceivableSnapshots)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ar_snapshots_branch");

            entity.HasOne(d => d.Customer).WithMany(p => p.AccountsReceivableSnapshots)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ar_snapshots_customer");

            entity.HasOne(d => d.Region).WithMany(p => p.AccountsReceivableSnapshots)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ar_snapshots_region");

            entity.HasOne(d => d.Tenant).WithMany(p => p.AccountsReceivableSnapshots)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_ar_snapshots_tenant");
        });

        modelBuilder.Entity<ActAuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("act_audit_logs_pkey");

            entity.ToTable("act_audit_logs", "notarial", tb => tb.HasComment("Evidence-grade audit log for every significant act change."));

            entity.Property(e => e.AuditLogId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Act).WithMany(p => p.ActAuditLogs).HasConstraintName("fk_act_audit_logs_act");

            entity.HasOne(d => d.ActorNotary).WithMany(p => p.ActAuditLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_act_audit_logs_actor_notary");

            entity.HasOne(d => d.ActorUser).WithMany(p => p.ActAuditLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_act_audit_logs_actor_user");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ActAuditLogs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_act_audit_logs_tenant");
        });

        modelBuilder.Entity<ActDocument>(entity =>
        {
            entity.HasKey(e => e.ActDocumentId).HasName("act_documents_pkey");

            entity.ToTable("act_documents", "notarial", tb => tb.HasComment("Supporting documents and uploaded evidence for an act."));

            entity.Property(e => e.ActDocumentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ChecksumSha256).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsSensitive).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StorageProvider).HasDefaultValueSql("'object_storage'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VisibilityLevel).HasDefaultValueSql("'restricted'::character varying");

            entity.HasOne(d => d.Act).WithMany(p => p.ActDocuments).HasConstraintName("fk_act_documents_act");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ActDocuments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_act_documents_tenant");

            entity.HasOne(d => d.UploadedByUser).WithMany(p => p.ActDocuments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_act_documents_uploaded_by");
        });

        modelBuilder.Entity<ActExecutionRecord>(entity =>
        {
            entity.HasKey(e => e.ExecutionRecordId).HasName("act_execution_records_pkey");

            entity.ToTable("act_execution_records", "notarial", tb => tb.HasComment("Execution-stage legal record, including oath and appearance confirmation."));

            entity.Property(e => e.ExecutionRecordId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.ExceptionFlags).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OathAdministered).HasDefaultValue(false);
            entity.Property(e => e.PersonalAppearanceConfirmed).HasDefaultValue(false);
            entity.Property(e => e.SignatureCaptured).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Act).WithMany(p => p.ActExecutionRecords).HasConstraintName("fk_act_execution_records_act");

            entity.HasOne(d => d.ExecutedByNotary).WithMany(p => p.ActExecutionRecords)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_act_execution_records_executed_by_notary");

            entity.HasOne(d => d.ExecutedByUser).WithMany(p => p.ActExecutionRecords)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_act_execution_records_executed_by_user");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ActExecutionRecords)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_act_execution_records_tenant");
        });

        modelBuilder.Entity<ActIdentityVerification>(entity =>
        {
            entity.HasKey(e => e.VerificationId).HasName("act_identity_verifications_pkey");

            entity.ToTable("act_identity_verifications", "notarial", tb => tb.HasComment("Identity verification steps and evidence for signers."));

            entity.Property(e => e.VerificationId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.KbaAttemptCount).HasDefaultValue(0);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Act).WithMany(p => p.ActIdentityVerifications).HasConstraintName("fk_act_identity_verifications_act");

            entity.HasOne(d => d.ActSigner).WithMany(p => p.ActIdentityVerifications).HasConstraintName("fk_act_identity_verifications_signer");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ActIdentityVerifications)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_act_identity_verifications_tenant");

            entity.HasOne(d => d.VerifiedByNotary).WithMany(p => p.ActIdentityVerifications)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_act_identity_verifications_verified_by_notary");

            entity.HasOne(d => d.VerifiedByUser).WithMany(p => p.ActIdentityVerifications)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_act_identity_verifications_verified_by_user");
        });

        modelBuilder.Entity<ActSigner>(entity =>
        {
            entity.HasKey(e => e.ActSignerId).HasName("act_signers_pkey");

            entity.ToTable("act_signers", "notarial", tb => tb.HasComment("One or more signers associated with a notarial act."));

            entity.HasIndex(e => e.ActId, "ux_act_signers_one_primary")
                .IsUnique()
                .HasFilter("((is_primary_signer = true) AND (deleted_at IS NULL))");

            entity.Property(e => e.ActSignerId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.AppearanceConfirmed).HasDefaultValue(false);
            entity.Property(e => e.CountryCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsPrimarySigner).HasDefaultValue(false);
            entity.Property(e => e.IsPrincipal).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.SignatureCaptured).HasDefaultValue(false);
            entity.Property(e => e.SignatureRequired).HasDefaultValue(true);
            entity.Property(e => e.SignerOrder).HasDefaultValue(1);
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.ThumbprintCaptured).HasDefaultValue(false);
            entity.Property(e => e.ThumbprintRequired).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Act).WithOne(p => p.ActSigner).HasConstraintName("fk_act_signers_act");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ActSigners)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_act_signers_tenant");
        });

        modelBuilder.Entity<ActStatusHistory>(entity =>
        {
            entity.HasKey(e => e.ActStatusHistoryId).HasName("act_status_history_pkey");

            entity.ToTable("act_status_history", "notarial", tb => tb.HasComment("Immutable status transitions for legal traceability."));

            entity.Property(e => e.ActStatusHistoryId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.EffectiveAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");

            entity.HasOne(d => d.Act).WithMany(p => p.ActStatusHistories).HasConstraintName("fk_act_status_history_act");

            entity.HasOne(d => d.ChangedByNotary).WithMany(p => p.ActStatusHistories)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_act_status_history_changed_by_notary");

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.ActStatusHistories)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_act_status_history_changed_by_user");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ActStatusHistories)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_act_status_history_tenant");
        });

        modelBuilder.Entity<BillingAdjustment>(entity =>
        {
            entity.HasKey(e => e.BillingAdjustmentId).HasName("billing_adjustments_pkey");

            entity.ToTable("billing_adjustments", "billing", tb => tb.HasComment("Manual billing adjustments and corrections."));

            entity.Property(e => e.BillingAdjustmentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.AdjustmentDate).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.CurrencyCode)
                .HasDefaultValueSql("'USD'::bpchar")
                .IsFixedLength();
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.BillingAdjustmentcreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_billing_adjustments_created_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.BillingAdjustments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_billing_adjustments_customer");

            entity.HasOne(d => d.Invoice).WithMany(p => p.BillingAdjustments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_billing_adjustments_invoice");

            entity.HasOne(d => d.Payment).WithMany(p => p.BillingAdjustments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_billing_adjustments_payment");

            entity.HasOne(d => d.Tenant).WithMany(p => p.BillingAdjustments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_billing_adjustments_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.BillingAdjustmentupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_billing_adjustments_updated_by");
        });

        modelBuilder.Entity<BillingAuditLog>(entity =>
        {
            entity.HasKey(e => e.BillingAuditLogId).HasName("billing_audit_logs_pkey");

            entity.ToTable("billing_audit_logs", "billing", tb => tb.HasComment("Evidence-grade billing audit trail."));

            entity.Property(e => e.BillingAuditLogId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ActorUser).WithMany(p => p.BillingAuditLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_billing_audit_logs_actor_user");

            entity.HasOne(d => d.Tenant).WithMany(p => p.BillingAuditLogs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_billing_audit_logs_tenant");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("branches_pkey");

            entity.ToTable("branches", "core", tb => tb.HasComment("Operational branches/offices."));

            entity.Property(e => e.BranchId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CountryCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Settings).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.TimeZone).HasDefaultValueSql("'UTC'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Organization).WithMany(p => p.Branches)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_branches_organization");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Branches)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_branches_tenant");
        });

        modelBuilder.Entity<CallLog>(entity =>
        {
            entity.HasKey(e => e.CallLogId).HasName("call_logs_pkey");

            entity.ToTable("call_logs", "communication", tb => tb.HasComment("Call records, outcomes, and optional recordings."));

            entity.Property(e => e.CallLogId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CalleeNotary).WithMany(p => p.CallLogcalleeNotaries)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_call_logs_callee_notary");

            entity.HasOne(d => d.CalleeParticipant).WithMany(p => p.CallLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_call_logs_callee_participant");

            entity.HasOne(d => d.CalleeUser).WithMany(p => p.CallLogcalleeUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_call_logs_callee_user");

            entity.HasOne(d => d.CallerNotary).WithMany(p => p.CallLogcallerNotaries)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_call_logs_caller_notary");

            entity.HasOne(d => d.CallerUser).WithMany(p => p.CallLogcallerUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_call_logs_caller_user");

            entity.HasOne(d => d.Message).WithMany(p => p.CallLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_call_logs_message");

            entity.HasOne(d => d.Reminder).WithMany(p => p.CallLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_call_logs_reminder");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CallLogs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_call_logs_tenant");

            entity.HasOne(d => d.Thread).WithMany(p => p.CallLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_call_logs_thread");
        });

        modelBuilder.Entity<CommunicationAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("communication_attachments_pkey");

            entity.ToTable("communication_attachments", "communication", tb => tb.HasComment("Files attached to messages or threads."));

            entity.Property(e => e.AttachmentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ChecksumSha256).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsSensitive).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StorageProvider).HasDefaultValueSql("'object_storage'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VisibilityLevel).HasDefaultValueSql("'restricted'::character varying");

            entity.HasOne(d => d.Message).WithMany(p => p.CommunicationAttachments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_attachments_message");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CommunicationAttachments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_attachments_tenant");

            entity.HasOne(d => d.Thread).WithMany(p => p.CommunicationAttachments).HasConstraintName("fk_attachments_thread");

            entity.HasOne(d => d.UploadedByUser).WithMany(p => p.CommunicationAttachments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_attachments_uploaded_by");
        });

        modelBuilder.Entity<CommunicationAuditLog>(entity =>
        {
            entity.HasKey(e => e.CommunicationAuditLogId).HasName("communication_audit_logs_pkey");

            entity.ToTable("communication_audit_logs", "communication", tb => tb.HasComment("Evidence-grade audit trail for communication operations."));

            entity.Property(e => e.CommunicationAuditLogId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ActorNotary).WithMany(p => p.CommunicationAuditLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_communication_audit_logs_actor_notary");

            entity.HasOne(d => d.ActorUser).WithMany(p => p.CommunicationAuditLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_communication_audit_logs_actor_user");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CommunicationAuditLogs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_communication_audit_logs_tenant");
        });

        modelBuilder.Entity<CommunicationDeliveryLog>(entity =>
        {
            entity.HasKey(e => e.DeliveryLogId).HasName("communication_delivery_logs_pkey");

            entity.ToTable("communication_delivery_logs", "communication", tb => tb.HasComment("Delivery attempts and provider responses."));

            entity.Property(e => e.DeliveryLogId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.RequestedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.ResponsePayload).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Message).WithMany(p => p.CommunicationDeliveryLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_delivery_logs_message");

            entity.HasOne(d => d.Reminder).WithMany(p => p.CommunicationDeliveryLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_delivery_logs_reminder");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CommunicationDeliveryLogs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_delivery_logs_tenant");

            entity.HasOne(d => d.Thread).WithMany(p => p.CommunicationDeliveryLogs).HasConstraintName("fk_delivery_logs_thread");
        });

        modelBuilder.Entity<CommunicationMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("communication_messages_pkey");

            entity.ToTable("communication_messages", "communication", tb => tb.HasComment("Individual inbound/outbound/internal messages."));

            entity.Property(e => e.MessageId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsImportant).HasDefaultValue(false);
            entity.Property(e => e.IsInternal).HasDefaultValue(false);
            entity.Property(e => e.IsSystemGenerated).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.CommunicationMessagecreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_messages_created_by");

            entity.HasOne(d => d.RecipientParticipant).WithMany(p => p.CommunicationMessagerecipientParticipants)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_messages_recipient_participant");

            entity.HasOne(d => d.ReplyToMessage).WithMany(p => p.InversereplyToMessage)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_messages_reply_to");

            entity.HasOne(d => d.SenderNotary).WithMany(p => p.CommunicationMessages)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_messages_sender_notary");

            entity.HasOne(d => d.SenderParticipant).WithMany(p => p.CommunicationMessagesenderParticipants)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_messages_sender_participant");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.CommunicationMessagesenderUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_messages_sender_user");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CommunicationMessages)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_messages_tenant");

            entity.HasOne(d => d.Thread).WithMany(p => p.CommunicationMessages).HasConstraintName("fk_messages_thread");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.CommunicationMessageupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_messages_updated_by");
        });

        modelBuilder.Entity<CommunicationParticipant>(entity =>
        {
            entity.HasKey(e => e.ParticipantId).HasName("communication_participants_pkey");

            entity.ToTable("communication_participants", "communication", tb => tb.HasComment("Participants in a communication thread."));

            entity.HasIndex(e => e.ThreadId, "ux_participants_one_primary")
                .IsUnique()
                .HasFilter("((is_primary = true) AND (deleted_at IS NULL))");

            entity.Property(e => e.ParticipantId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.JoinedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.NotificationEmail).HasDefaultValue(true);
            entity.Property(e => e.NotificationInApp).HasDefaultValue(true);
            entity.Property(e => e.NotificationSms).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CustomerContact).WithMany(p => p.CommunicationParticipants)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_participants_customer_contact");

            entity.HasOne(d => d.Customer).WithMany(p => p.CommunicationParticipants)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_participants_customer");

            entity.HasOne(d => d.Notary).WithMany(p => p.CommunicationParticipants)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_participants_notary");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CommunicationParticipants)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_participants_tenant");

            entity.HasOne(d => d.Thread).WithOne(p => p.CommunicationParticipant).HasConstraintName("fk_participants_thread");

            entity.HasOne(d => d.User).WithMany(p => p.CommunicationParticipants)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_participants_user");
        });

        modelBuilder.Entity<CommunicationReminder>(entity =>
        {
            entity.HasKey(e => e.ReminderId).HasName("communication_reminders_pkey");

            entity.ToTable("communication_reminders", "communication", tb => tb.HasComment("Scheduled reminders and notifications."));

            entity.Property(e => e.ReminderId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.Payload).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.CommunicationRemindercreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_reminders_created_by");

            entity.HasOne(d => d.Invoice).WithMany(p => p.CommunicationReminders)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_reminders_invoice");

            entity.HasOne(d => d.Job).WithMany(p => p.CommunicationReminders)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_reminders_job");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.CommunicationReminders)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_reminders_journal_entry");

            entity.HasOne(d => d.Message).WithMany(p => p.CommunicationReminders)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_reminders_message");

            entity.HasOne(d => d.NotarialAct).WithMany(p => p.CommunicationReminders)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_reminders_notarial_act");

            entity.HasOne(d => d.RecipientNotary).WithMany(p => p.CommunicationReminders)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_reminders_recipient_notary");

            entity.HasOne(d => d.RecipientParticipant).WithMany(p => p.CommunicationReminders)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_reminders_recipient_participant");

            entity.HasOne(d => d.RecipientUser).WithMany(p => p.CommunicationReminderrecipientUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_reminders_recipient_user");

            entity.HasOne(d => d.Template).WithMany(p => p.CommunicationReminders)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_reminders_template");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CommunicationReminders)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_reminders_tenant");

            entity.HasOne(d => d.Thread).WithMany(p => p.CommunicationReminders)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_reminders_thread");
        });

        modelBuilder.Entity<CommunicationTemplate>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("communication_templates_pkey");

            entity.ToTable("communication_templates", "communication", tb => tb.HasComment("Reusable templates for email, SMS, call scripts, and meetings."));

            entity.Property(e => e.TemplateId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSystem).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VariablesSchema).HasDefaultValueSql("'[]'::jsonb");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.CommunicationTemplatecreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_templates_created_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CommunicationTemplates)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_templates_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.CommunicationTemplateupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_templates_updated_by");
        });

        modelBuilder.Entity<CommunicationThread>(entity =>
        {
            entity.HasKey(e => e.ThreadId).HasName("communication_threads_pkey");

            entity.ToTable("communication_threads", "communication", tb => tb.HasComment("Conversation/thread aggregate tied to customer, job, act, invoice, or incident."));

            entity.Property(e => e.ThreadId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsImportant).HasDefaultValue(false);
            entity.Property(e => e.IsInternal).HasDefaultValue(false);
            entity.Property(e => e.IsPinned).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.AssignedTeam).WithMany(p => p.CommunicationThreads)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_assigned_team");

            entity.HasOne(d => d.AssignedUser).WithMany(p => p.CommunicationThreadassignedUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_assigned_user");

            entity.HasOne(d => d.Branch).WithMany(p => p.CommunicationThreads)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_branch");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.CommunicationThreadcreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_created_by");

            entity.HasOne(d => d.CustomerContact).WithMany(p => p.CommunicationThreads)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_customer_contact");

            entity.HasOne(d => d.Customer).WithMany(p => p.CommunicationThreads)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_customer");

            entity.HasOne(d => d.Incident).WithMany(p => p.CommunicationThreads)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_incident");

            entity.HasOne(d => d.Invoice).WithMany(p => p.CommunicationThreads)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_invoice");

            entity.HasOne(d => d.Job).WithMany(p => p.CommunicationThreads)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_job");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.CommunicationThreads)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_journal_entry");

            entity.HasOne(d => d.NotarialAct).WithMany(p => p.CommunicationThreads)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_notarial_act");

            entity.HasOne(d => d.Payment).WithMany(p => p.CommunicationThreads)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_payment");

            entity.HasOne(d => d.Region).WithMany(p => p.CommunicationThreads)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_region");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CommunicationThreads)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_threads_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.CommunicationThreadupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_threads_updated_by");
        });

        modelBuilder.Entity<ComplianceAuditEvent>(entity =>
        {
            entity.HasKey(e => e.ComplianceAuditEventId).HasName("compliance_audit_events_pkey");

            entity.ToTable("compliance_audit_events", "compliance", tb => tb.HasComment("Evidence-grade audit trail for compliance operations."));

            entity.Property(e => e.ComplianceAuditEventId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ActorNotary).WithMany(p => p.ComplianceAuditEvents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_audit_events_actor_notary");

            entity.HasOne(d => d.ActorUser).WithMany(p => p.ComplianceAuditEvents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_audit_events_actor_user");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ComplianceAuditEvents)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_compliance_audit_events_tenant");
        });

        modelBuilder.Entity<ComplianceAuditLog>(entity =>
        {
            entity.HasKey(e => e.ComplianceAuditLogId).HasName("compliance_audit_logs_pkey");

            entity.ToTable("compliance_audit_logs", "compliance", tb => tb.HasComment("Unified audit log for compliance operations."));

            entity.Property(e => e.ComplianceAuditLogId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ActorNotary).WithMany(p => p.ComplianceAuditLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_audit_logs_actor_notary");

            entity.HasOne(d => d.ActorUser).WithMany(p => p.ComplianceAuditLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_audit_logs_actor_user");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ComplianceAuditLogs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_compliance_audit_logs_tenant");
        });

        modelBuilder.Entity<ComplianceCheck>(entity =>
        {
            entity.HasKey(e => e.ComplianceCheckId).HasName("compliance_checks_pkey");

            entity.ToTable("compliance_checks", "compliance", tb => tb.HasComment("Compliance evaluation runs."));

            entity.Property(e => e.ComplianceCheckId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.ResultSummary).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Act).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_checks_act");

            entity.HasOne(d => d.Branch).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_checks_branch");

            entity.HasOne(d => d.Certificate).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_checks_certificate");

            entity.HasOne(d => d.Customer).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_checks_customer");

            entity.HasOne(d => d.Job).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_checks_job");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_checks_journal_entry");

            entity.HasOne(d => d.PolicyVersion).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_checks_policy_version");

            entity.HasOne(d => d.Region).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_checks_region");

            entity.HasOne(d => d.Seal).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_checks_seal");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_checks_service_type");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_compliance_checks_tenant");

            entity.HasOne(d => d.TriggeredByNotary).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_checks_triggered_by_notary");

            entity.HasOne(d => d.TriggeredByUser).WithMany(p => p.ComplianceChecks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_checks_triggered_by_user");
        });

        modelBuilder.Entity<ComplianceCheckResult>(entity =>
        {
            entity.HasKey(e => e.ComplianceCheckResultId).HasName("compliance_check_results_pkey");

            entity.ToTable("compliance_check_results", "compliance", tb => tb.HasComment("Detailed findings for compliance checks."));

            entity.Property(e => e.ComplianceCheckResultId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.EvaluatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Evidence).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.IsBlocking).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.RequiresManualReview).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ComplianceCheck).WithMany(p => p.ComplianceCheckResults).HasConstraintName("fk_compliance_check_results_check");

            entity.HasOne(d => d.EvaluatedByNotary).WithMany(p => p.ComplianceCheckResults)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_check_results_evaluated_by_notary");

            entity.HasOne(d => d.EvaluatedByUser).WithMany(p => p.ComplianceCheckResults)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_check_results_evaluated_by_user");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ComplianceCheckResults)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_compliance_check_results_tenant");
        });

        modelBuilder.Entity<ComplianceRule>(entity =>
        {
            entity.HasKey(e => e.ComplianceRuleId).HasName("compliance_rules_pkey");

            entity.ToTable("compliance_rules", "compliance", tb => tb.HasComment("Compliance rule catalog used for validation and blocking."));

            entity.Property(e => e.ComplianceRuleId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.BlockOnFailure).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsMandatory).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.RequiresManualReview).HasDefaultValue(false);
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Act).WithMany(p => p.ComplianceRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_act");

            entity.HasOne(d => d.Branch).WithMany(p => p.ComplianceRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_branch");

            entity.HasOne(d => d.Certificate).WithMany(p => p.ComplianceRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_certificate");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ComplianceRulecreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_created_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.ComplianceRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_customer");

            entity.HasOne(d => d.Job).WithMany(p => p.ComplianceRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_job");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.ComplianceRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_journal_entry");

            entity.HasOne(d => d.Notary).WithMany(p => p.ComplianceRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_notary");

            entity.HasOne(d => d.Region).WithMany(p => p.ComplianceRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_region");

            entity.HasOne(d => d.Role).WithMany(p => p.ComplianceRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_role");

            entity.HasOne(d => d.Seal).WithMany(p => p.ComplianceRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_seal");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.ComplianceRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_service_type");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ComplianceRules)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_compliance_rules_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.ComplianceRuleupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_compliance_rules_updated_by");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("contracts_pkey");

            entity.ToTable("contracts", "crm", tb => tb.HasComment("Commercial contracts with B2B customers."));

            entity.Property(e => e.ContractId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.AutoRenew).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.RenewalNoticeDays).HasDefaultValue(30);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ContractcreatedByUsers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_contracts_created_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.Contracts).HasConstraintName("fk_contracts_customer");

            entity.HasOne(d => d.PricingPlan).WithMany(p => p.Contracts)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_contracts_pricing_plan");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Contracts)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_contracts_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.ContractupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_contracts_updated_by");
        });

        modelBuilder.Entity<ContractDocument>(entity =>
        {
            entity.HasKey(e => e.ContractDocumentId).HasName("contract_documents_pkey");

            entity.ToTable("contract_documents", "crm", tb => tb.HasComment("Uploaded contract files and annexes."));

            entity.Property(e => e.ContractDocumentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ChecksumSha256).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StorageProvider).HasDefaultValueSql("'object_storage'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VersionNo).HasDefaultValue(1);

            entity.HasOne(d => d.Contract).WithMany(p => p.ContractDocuments).HasConstraintName("fk_contract_documents_contract");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ContractDocuments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_contract_documents_tenant");

            entity.HasOne(d => d.UploadedByUser).WithMany(p => p.ContractDocuments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_contract_documents_uploaded_by");
        });

        modelBuilder.Entity<Credit>(entity =>
        {
            entity.HasKey(e => e.CreditId).HasName("credits_pkey");

            entity.ToTable("credits", "billing", tb => tb.HasComment("Customer credit balances and credit memos."));

            entity.Property(e => e.CreditId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.CreditDate).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.CurrencyCode)
                .HasDefaultValueSql("'USD'::bpchar")
                .IsFixedLength();
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.CreditcreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_credits_created_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.Credits)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_credits_customer");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Credits)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_credits_invoice");

            entity.HasOne(d => d.OriginPayment).WithMany(p => p.Credits)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_credits_origin_payment");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Credits)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_credits_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.CreditupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_credits_updated_by");
        });

        modelBuilder.Entity<CreditApplication>(entity =>
        {
            entity.HasKey(e => e.CreditApplicationId).HasName("credit_applications_pkey");

            entity.ToTable("credit_applications", "billing", tb => tb.HasComment("Application of credits to invoices."));

            entity.Property(e => e.CreditApplicationId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.AppliedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Credit).WithMany(p => p.CreditApplications).HasConstraintName("fk_credit_applications_credit");

            entity.HasOne(d => d.Invoice).WithMany(p => p.CreditApplications).HasConstraintName("fk_credit_applications_invoice");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CreditApplications)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_credit_applications_tenant");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("customers_pkey");

            entity.ToTable("customers", "crm", tb => tb.HasComment("Core customer/account entity for B2C and B2B."));

            entity.Property(e => e.CustomerId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CountryCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Settings).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.TagsSummary).HasDefaultValueSql("'[]'::jsonb");
            entity.Property(e => e.TotalJobs).HasDefaultValue(0);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.AccountManagerUser).WithMany(p => p.Customers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_customers_account_manager");

            entity.HasOne(d => d.Branch).WithMany(p => p.Customers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_customers_branch");

            entity.HasOne(d => d.Region).WithMany(p => p.Customers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_customers_region");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Customers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_customers_tenant");
        });

        modelBuilder.Entity<CustomerContact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("customer_contacts_pkey");

            entity.ToTable("customer_contacts", "crm", tb => tb.HasComment("Multiple contacts per customer/account."));

            entity.HasIndex(e => e.CustomerId, "ux_customer_contacts_one_primary")
                .IsUnique()
                .HasFilter("((is_primary = true) AND (deleted_at IS NULL))");

            entity.Property(e => e.ContactId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.Settings).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Customer).WithOne(p => p.CustomerContact).HasConstraintName("fk_customer_contacts_customer");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CustomerContacts)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_customer_contacts_tenant");
        });

        modelBuilder.Entity<CustomerContactPreference>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("customer_contact_preferences_pkey");

            entity.ToTable("customer_contact_preferences", "crm", tb => tb.HasComment("Notification and communication preferences for contacts."));

            entity.Property(e => e.ContactId).ValueGeneratedNever();
            entity.Property(e => e.BillingEnabled).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.EmailEnabled).HasDefaultValue(true);
            entity.Property(e => e.InAppEnabled).HasDefaultValue(true);
            entity.Property(e => e.MarketingEnabled).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OperationalEnabled).HasDefaultValue(true);
            entity.Property(e => e.PreferredLanguage).HasDefaultValueSql("'en'::character varying");
            entity.Property(e => e.SmsEnabled).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Contact).WithOne(p => p.CustomerContactPreference).HasConstraintName("fk_contact_preferences_contact");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CustomerContactPreferences)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_contact_preferences_tenant");
        });

        modelBuilder.Entity<CustomerDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("customer_documents_pkey");

            entity.ToTable("customer_documents", "crm", tb => tb.HasComment("Customer-related documents and files."));

            entity.Property(e => e.DocumentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ChecksumSha256).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsSensitive).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StorageProvider).HasDefaultValueSql("'object_storage'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VisibilityLevel).HasDefaultValueSql("'restricted'::character varying");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerDocuments).HasConstraintName("fk_customer_documents_customer");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CustomerDocuments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_customer_documents_tenant");

            entity.HasOne(d => d.UploadedByUser).WithMany(p => p.CustomerDocumentuploadedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_customer_documents_uploaded_by");

            entity.HasOne(d => d.VerifiedByUser).WithMany(p => p.CustomerDocumentverifiedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_customer_documents_verified_by");
        });

        modelBuilder.Entity<CustomerNote>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("customer_notes_pkey");

            entity.ToTable("customer_notes", "crm", tb => tb.HasComment("Internal notes and compliance notes."));

            entity.Property(e => e.NoteId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsComplianceNote).HasDefaultValue(false);
            entity.Property(e => e.Pinned).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.CustomerNotecreatedByUsers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_customer_notes_created_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerNotes).HasConstraintName("fk_customer_notes_customer");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CustomerNotes)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_customer_notes_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.CustomerNoteupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_customer_notes_updated_by");
        });

        modelBuilder.Entity<CustomerSegment>(entity =>
        {
            entity.HasKey(e => e.SegmentId).HasName("customer_segments_pkey");

            entity.ToTable("customer_segments", "crm", tb => tb.HasComment("Segmentation catalog."));

            entity.Property(e => e.SegmentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CustomerSegments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_customer_segments_tenant");
        });

        modelBuilder.Entity<CustomerSegmentAssignment>(entity =>
        {
            entity.HasKey(e => new { customer_id = e.CustomerId, segment_id = e.SegmentId }).HasName("customer_segment_assignments_pkey");

            entity.ToTable("customer_segment_assignments", "crm", tb => tb.HasComment("Customer-to-segment mapping."));

            entity.Property(e => e.AssignedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.AssignedByUser).WithMany(p => p.CustomerSegmentAssignments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_customer_segment_assignments_assigned_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerSegmentAssignments).HasConstraintName("fk_customer_segment_assignments_customer");

            entity.HasOne(d => d.Segment).WithMany(p => p.CustomerSegmentAssignments).HasConstraintName("fk_customer_segment_assignments_segment");
        });

        modelBuilder.Entity<CustomerStatusHistory>(entity =>
        {
            entity.HasKey(e => e.StatusHistoryId).HasName("customer_status_history_pkey");

            entity.ToTable("customer_status_history", "crm", tb => tb.HasComment("Status transition history for audit and compliance."));

            entity.Property(e => e.StatusHistoryId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.EffectiveAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.CustomerStatusHistories)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_customer_status_history_changed_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerStatusHistories).HasConstraintName("fk_customer_status_history_customer");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CustomerStatusHistories)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_customer_status_history_tenant");
        });

        modelBuilder.Entity<CustomerTag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("customer_tags_pkey");

            entity.ToTable("customer_tags", "crm", tb => tb.HasComment("Tag catalog for segmentation."));

            entity.Property(e => e.TagId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSystem).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Tenant).WithMany(p => p.CustomerTags)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_customer_tags_tenant");
        });

        modelBuilder.Entity<CustomerTagAssignment>(entity =>
        {
            entity.HasKey(e => new { customer_id = e.CustomerId, tag_id = e.TagId }).HasName("customer_tag_assignments_pkey");

            entity.ToTable("customer_tag_assignments", "crm", tb => tb.HasComment("Customer-to-tag mapping."));

            entity.Property(e => e.AssignedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.AssignedByUser).WithMany(p => p.CustomerTagAssignments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_customer_tag_assignments_assigned_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerTagAssignments).HasConstraintName("fk_customer_tag_assignments_customer");

            entity.HasOne(d => d.Tag).WithMany(p => p.CustomerTagAssignments).HasConstraintName("fk_customer_tag_assignments_tag");
        });

        modelBuilder.Entity<DailyOperationalSnapshot>(entity =>
        {
            entity.HasKey(e => e.DailySnapshotId).HasName("daily_operational_snapshots_pkey");

            entity.ToTable("daily_operational_snapshots", "operations", tb => tb.HasComment("Daily operational dashboard aggregates."));

            entity.Property(e => e.DailySnapshotId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CancelledJobs).HasDefaultValue(0);
            entity.Property(e => e.CompletedJobs).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.FailedJobs).HasDefaultValue(0);
            entity.Property(e => e.Metrics).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OnTimeJobs).HasDefaultValue(0);
            entity.Property(e => e.ReassignedJobs).HasDefaultValue(0);
            entity.Property(e => e.TotalDispatchRuns).HasDefaultValue(0);
            entity.Property(e => e.TotalJobs).HasDefaultValue(0);
            entity.Property(e => e.TotalRemindersSent).HasDefaultValue(0);
            entity.Property(e => e.TotalRequests).HasDefaultValue(0);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Branch).WithMany(p => p.DailyOperationalSnapshots)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_daily_operational_snapshots_branch");

            entity.HasOne(d => d.Region).WithMany(p => p.DailyOperationalSnapshots)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_daily_operational_snapshots_region");

            entity.HasOne(d => d.Tenant).WithMany(p => p.DailyOperationalSnapshots)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_daily_operational_snapshots_tenant");
        });

        modelBuilder.Entity<DigitalCertificate>(entity =>
        {
            entity.HasKey(e => e.DigitalCertificateId).HasName("digital_certificates_pkey");

            entity.ToTable("digital_certificates", "security", tb => tb.HasComment("Digital certificates used for electronic seals and signature workflows."));

            entity.Property(e => e.DigitalCertificateId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CertificateChain).HasDefaultValueSql("'[]'::jsonb");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.KeyRotationStatus).HasDefaultValueSql("'not_rotated'::character varying");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.ThumbprintSha1).IsFixedLength();
            entity.Property(e => e.ThumbprintSha256).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.DigitalCertificatecreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_digital_certificates_created_by");

            entity.HasOne(d => d.Notary).WithMany(p => p.DigitalCertificates)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_digital_certificates_notary");

            entity.HasOne(d => d.Seal).WithMany(p => p.DigitalCertificates)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_digital_certificates_seal");

            entity.HasOne(d => d.Tenant).WithMany(p => p.DigitalCertificates)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_digital_certificates_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.DigitalCertificateupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_digital_certificates_updated_by");
        });

        modelBuilder.Entity<DigitalCertificateChainItem>(entity =>
        {
            entity.HasKey(e => e.ChainItemId).HasName("digital_certificate_chain_items_pkey");

            entity.ToTable("digital_certificate_chain_items", "security", tb => tb.HasComment("Certificate chain items for validation and audit."));

            entity.Property(e => e.ChainItemId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsRoot).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.ThumbprintSha256).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.DigitalCertificate).WithMany(p => p.DigitalCertificateChainItems).HasConstraintName("fk_chain_items_certificate");

            entity.HasOne(d => d.Tenant).WithMany(p => p.DigitalCertificateChainItems)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_chain_items_tenant");
        });

        modelBuilder.Entity<DispatchCandidate>(entity =>
        {
            entity.HasKey(e => e.DispatchCandidateId).HasName("dispatch_candidates_pkey");

            entity.ToTable("dispatch_candidates", "operations", tb => tb.HasComment("Candidate notaries per dispatch run."));

            entity.HasIndex(e => new { dispatch_run_id = e.DispatchRunId, notary_id = e.NotaryId }, "ux_dispatch_candidates_run_notary")
                .IsUnique()
                .HasFilter("(deleted_at IS NULL)");

            entity.Property(e => e.DispatchCandidateId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.AvailabilitySnapshot).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.ComplianceSnapshot).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsSelected).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Branch).WithMany(p => p.DispatchCandidates)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_dispatch_candidates_branch");

            entity.HasOne(d => d.DispatchRun).WithMany(p => p.DispatchCandidates).HasConstraintName("fk_dispatch_candidates_run");

            entity.HasOne(d => d.Notary).WithMany(p => p.DispatchCandidates)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_dispatch_candidates_notary");

            entity.HasOne(d => d.Region).WithMany(p => p.DispatchCandidates)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_dispatch_candidates_region");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.DispatchCandidates)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_dispatch_candidates_service_type");

            entity.HasOne(d => d.Tenant).WithMany(p => p.DispatchCandidates)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_dispatch_candidates_tenant");
        });

        modelBuilder.Entity<DispatchRule>(entity =>
        {
            entity.HasKey(e => e.DispatchRuleId).HasName("dispatch_rules_pkey");

            entity.ToTable("dispatch_rules", "operations", tb => tb.HasComment("Rule catalog for dispatch logic."));

            entity.Property(e => e.DispatchRuleId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Actions).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.Conditions).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.Priority).HasDefaultValue(100);
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Branch).WithMany(p => p.DispatchRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_dispatch_rules_branch");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.DispatchRulecreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_dispatch_rules_created_by");

            entity.HasOne(d => d.Region).WithMany(p => p.DispatchRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_dispatch_rules_region");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.DispatchRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_dispatch_rules_service_type");

            entity.HasOne(d => d.Tenant).WithMany(p => p.DispatchRules)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_dispatch_rules_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.DispatchRuleupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_dispatch_rules_updated_by");
        });

        modelBuilder.Entity<DispatchRun>(entity =>
        {
            entity.HasKey(e => e.DispatchRunId).HasName("dispatch_runs_pkey");

            entity.ToTable("dispatch_runs", "operations", tb => tb.HasComment("Dispatch evaluation runs and algorithm snapshots."));

            entity.Property(e => e.DispatchRunId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.InitiatedByUser).WithMany(p => p.DispatchRuns)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_dispatch_runs_initiated_by");

            entity.HasOne(d => d.JobRequest).WithMany(p => p.DispatchRuns).HasConstraintName("fk_dispatch_runs_job_request");

            entity.HasOne(d => d.Tenant).WithMany(p => p.DispatchRuns)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_dispatch_runs_tenant");
        });

        modelBuilder.Entity<EmergencyLock>(entity =>
        {
            entity.HasKey(e => e.EmergencyLockId).HasName("emergency_locks_pkey");

            entity.ToTable("emergency_locks", "security", tb => tb.HasComment("One-click suspension / emergency lock records."));

            entity.Property(e => e.EmergencyLockId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.LockedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.DigitalCertificate).WithMany(p => p.EmergencyLocks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_emergency_locks_certificate");

            entity.HasOne(d => d.Incident).WithMany(p => p.EmergencyLocks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_emergency_locks_incident");

            entity.HasOne(d => d.LockedByUser).WithMany(p => p.EmergencyLocklockedByUsers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_emergency_locks_locked_by");

            entity.HasOne(d => d.ReleasedByUser).WithMany(p => p.EmergencyLockreleasedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_emergency_locks_released_by");

            entity.HasOne(d => d.Seal).WithMany(p => p.EmergencyLocks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_emergency_locks_seal");

            entity.HasOne(d => d.Tenant).WithMany(p => p.EmergencyLocks)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_emergency_locks_tenant");
        });

        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(e => e.IncidentId).HasName("incidents_pkey");

            entity.ToTable("incidents", "compliance", tb => tb.HasComment("Compliance incidents and violations."));

            entity.Property(e => e.IncidentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.DetectedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.LegalHoldApplied).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.RegulatoryNotificationRequired).HasDefaultValue(false);
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Act).WithMany(p => p.Incidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incidents_act");

            entity.HasOne(d => d.AssignedToUser).WithMany(p => p.IncidentassignedToUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incidents_assigned_to");

            entity.HasOne(d => d.Branch).WithMany(p => p.Incidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incidents_branch");

            entity.HasOne(d => d.Certificate).WithMany(p => p.Incidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incidents_certificate");

            entity.HasOne(d => d.Customer).WithMany(p => p.Incidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incidents_customer");

            entity.HasOne(d => d.Job).WithMany(p => p.Incidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incidents_job");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.Incidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incidents_journal_entry");

            entity.HasOne(d => d.LegalHold).WithMany(p => p.Incidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incidents_legal_hold");

            entity.HasOne(d => d.Region).WithMany(p => p.Incidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incidents_region");

            entity.HasOne(d => d.ReportedByUser).WithMany(p => p.IncidentreportedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incidents_reported_by");

            entity.HasOne(d => d.Seal).WithMany(p => p.Incidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incidents_seal");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Incidents)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_incidents_tenant");
        });

        modelBuilder.Entity<IncidentAction>(entity =>
        {
            entity.HasKey(e => e.IncidentActionId).HasName("incident_actions_pkey");

            entity.ToTable("incident_actions", "compliance", tb => tb.HasComment("Corrective actions for incidents."));

            entity.Property(e => e.IncidentActionId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ActionStatus).HasDefaultValueSql("'open'::character varying");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.AssignedToUser).WithMany(p => p.IncidentActionassignedToUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incident_actions_assigned_to");

            entity.HasOne(d => d.Incident).WithMany(p => p.IncidentActions).HasConstraintName("fk_incident_actions_incident");

            entity.HasOne(d => d.PerformedByUser).WithMany(p => p.IncidentActionperformedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incident_actions_performed_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.IncidentActions)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_incident_actions_tenant");
        });

        modelBuilder.Entity<Inspection>(entity =>
        {
            entity.HasKey(e => e.InspectionId).HasName("inspections_pkey");

            entity.ToTable("inspections", "compliance", tb => tb.HasComment("Regulatory inspection tracking."));

            entity.Property(e => e.InspectionId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.InspectioncreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_inspections_created_by");

            entity.HasOne(d => d.LegalHold).WithMany(p => p.Inspections)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_inspections_legal_hold");

            entity.HasOne(d => d.RelatedExport).WithMany(p => p.Inspections)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_inspections_export");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Inspections)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_inspections_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.InspectionupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_inspections_updated_by");
        });

        modelBuilder.Entity<InternalNote>(entity =>
        {
            entity.HasKey(e => e.InternalNoteId).HasName("internal_notes_pkey");

            entity.ToTable("internal_notes", "communication", tb => tb.HasComment("Internal notes for sales, operations, and compliance."));

            entity.Property(e => e.InternalNoteId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsComplianceNote).HasDefaultValue(false);
            entity.Property(e => e.IsPinned).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.InternalNotecreatedByUsers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_internal_notes_created_by");

            entity.HasOne(d => d.CustomerContact).WithMany(p => p.InternalNotes)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_internal_notes_customer_contact");

            entity.HasOne(d => d.Customer).WithMany(p => p.InternalNotes)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_internal_notes_customer");

            entity.HasOne(d => d.Incident).WithMany(p => p.InternalNotes)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_internal_notes_incident");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InternalNotes)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_internal_notes_invoice");

            entity.HasOne(d => d.Job).WithMany(p => p.InternalNotes)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_internal_notes_job");

            entity.HasOne(d => d.NotarialAct).WithMany(p => p.InternalNotes)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_internal_notes_notarial_act");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InternalNotes)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_internal_notes_tenant");

            entity.HasOne(d => d.Thread).WithMany(p => p.InternalNotes)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_internal_notes_thread");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.InternalNoteupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_internal_notes_updated_by");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("invoices_pkey");

            entity.ToTable("invoices", "billing", tb => tb.HasComment("Customer invoices and AR head record."));

            entity.Property(e => e.InvoiceId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.AgingDays).HasDefaultValue(0);
            entity.Property(e => e.BillingCountryCode).IsFixedLength();
            entity.Property(e => e.BillingStateCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.CurrencyCode)
                .HasDefaultValueSql("'USD'::bpchar")
                .IsFixedLength();
            entity.Property(e => e.ExchangeRate).HasDefaultValueSql("1");
            entity.Property(e => e.InvoiceDate).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Branch).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_invoices_branch");

            entity.HasOne(d => d.Contract).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_invoices_contract");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.InvoicecreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_invoices_created_by");

            entity.HasOne(d => d.CustomerContact).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_invoices_customer_contact");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_invoices_customer");

            entity.HasOne(d => d.Region).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_invoices_region");

            entity.HasOne(d => d.Sla).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_invoices_sla");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_invoices_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.InvoiceupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_invoices_updated_by");
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.InvoiceItemId).HasName("invoice_items_pkey");

            entity.ToTable("invoice_items", "billing", tb => tb.HasComment("Invoice line items linked to jobs, acts, and journal entries."));

            entity.Property(e => e.InvoiceItemId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.Quantity).HasDefaultValueSql("1");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItems).HasConstraintName("fk_invoice_items_invoice");

            entity.HasOne(d => d.Job).WithMany(p => p.InvoiceItems)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_invoice_items_job");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.InvoiceItems)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_invoice_items_journal_entry");

            entity.HasOne(d => d.NotarialAct).WithMany(p => p.InvoiceItems)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_invoice_items_notarial_act");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.InvoiceItems)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_invoice_items_service_type");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvoiceItems)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_invoice_items_tenant");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("jobs_pkey");

            entity.ToTable("jobs", "operations", tb => tb.HasComment("Confirmed operational job records."));

            entity.Property(e => e.JobId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CountryCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.LockRequired).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.RushFlag).HasDefaultValue(false);
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.Timezone).HasDefaultValueSql("'UTC'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Branch).WithMany(p => p.Jobs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_jobs_branch");

            entity.HasOne(d => d.CancelledByUser).WithMany(p => p.JobcancelledByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_jobs_cancelled_by_user");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.JobcreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_jobs_created_by_user");

            entity.HasOne(d => d.CustomerContact).WithMany(p => p.Jobs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_jobs_customer_contact");

            entity.HasOne(d => d.Customer).WithMany(p => p.Jobs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_jobs_customer");

            entity.HasOne(d => d.JobRequest).WithMany(p => p.Jobs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_jobs_request");

            entity.HasOne(d => d.LockedByUser).WithMany(p => p.JoblockedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_jobs_locked_by_user");

            entity.HasOne(d => d.Region).WithMany(p => p.Jobs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_jobs_region");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.Jobs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_jobs_service_type");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Jobs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_jobs_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.JobupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_jobs_updated_by_user");
        });

        modelBuilder.Entity<JobAssignment>(entity =>
        {
            entity.HasKey(e => e.JobAssignmentId).HasName("job_assignments_pkey");

            entity.ToTable("job_assignments", "operations", tb => tb.HasComment("History of notary assignments for each job."));

            entity.HasIndex(e => e.JobId, "ux_job_assignments_one_primary")
                .IsUnique()
                .HasFilter("((is_primary = true) AND (deleted_at IS NULL))");

            entity.Property(e => e.JobAssignmentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ComplianceSnapshot).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.PerformanceSnapshot).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.ProposedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.AssignedByUser).WithMany(p => p.JobAssignmentassignedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_assignments_assigned_by");

            entity.HasOne(d => d.Job).WithOne(p => p.JobAssignment).HasConstraintName("fk_job_assignments_job");

            entity.HasOne(d => d.Notary).WithMany(p => p.JobAssignments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_job_assignments_notary");

            entity.HasOne(d => d.ReleasedByUser).WithMany(p => p.JobAssignmentreleasedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_assignments_released_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JobAssignments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_job_assignments_tenant");
        });

        modelBuilder.Entity<JobReminder>(entity =>
        {
            entity.HasKey(e => e.JobReminderId).HasName("job_reminders_pkey");

            entity.ToTable("job_reminders", "operations", tb => tb.HasComment("Operational reminders and delivery tracking."));

            entity.Property(e => e.JobReminderId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.Payload).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.JobRemindercreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_reminders_created_by");

            entity.HasOne(d => d.Job).WithMany(p => p.JobReminders).HasConstraintName("fk_job_reminders_job");

            entity.HasOne(d => d.RecipientContact).WithMany(p => p.JobReminders)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_reminders_recipient_contact");

            entity.HasOne(d => d.RecipientNotary).WithMany(p => p.JobReminders)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_reminders_recipient_notary");

            entity.HasOne(d => d.RecipientUser).WithMany(p => p.JobReminderrecipientUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_reminders_recipient_user");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JobReminders)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_job_reminders_tenant");
        });

        modelBuilder.Entity<JobRequest>(entity =>
        {
            entity.HasKey(e => e.JobRequestId).HasName("job_requests_pkey");

            entity.ToTable("job_requests", "operations", tb => tb.HasComment("Incoming job requests and orders."));

            entity.Property(e => e.JobRequestId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CountryCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.QuoteCurrencyCode)
                .HasDefaultValueSql("'USD'::bpchar")
                .IsFixedLength();
            entity.Property(e => e.RushFlag).HasDefaultValue(false);
            entity.Property(e => e.SourceChannel).HasDefaultValueSql("'internal'::character varying");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.Timezone).HasDefaultValueSql("'UTC'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.AssignedDispatcherUser).WithMany(p => p.JobRequestassignedDispatcherUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_requests_assigned_dispatcher");

            entity.HasOne(d => d.Branch).WithMany(p => p.JobRequests)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_requests_branch");

            entity.HasOne(d => d.CancelledByUser).WithMany(p => p.JobRequestcancelledByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_requests_cancelled_by");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.JobRequestcreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_requests_created_by_user");

            entity.HasOne(d => d.CustomerContact).WithMany(p => p.JobRequests)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_requests_customer_contact");

            entity.HasOne(d => d.Customer).WithMany(p => p.JobRequests)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_job_requests_customer");

            entity.HasOne(d => d.Region).WithMany(p => p.JobRequests)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_requests_region");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.JobRequests)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_job_requests_service_type");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JobRequests)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_job_requests_tenant");
        });

        modelBuilder.Entity<JobStatusHistory>(entity =>
        {
            entity.HasKey(e => e.JobStatusHistoryId).HasName("job_status_history_pkey");

            entity.ToTable("job_status_history", "operations", tb => tb.HasComment("Immutable status transition log for jobs."));

            entity.Property(e => e.JobStatusHistoryId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.EffectiveAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.JobStatusHistories)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_status_history_changed_by");

            entity.HasOne(d => d.Job).WithMany(p => p.JobStatusHistories).HasConstraintName("fk_job_status_history_job");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JobStatusHistories)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_job_status_history_tenant");
        });

        modelBuilder.Entity<JobTimelineEvent>(entity =>
        {
            entity.HasKey(e => e.JobTimelineEventId).HasName("job_timeline_events_pkey");

            entity.ToTable("job_timeline_events", "operations", tb => tb.HasComment("Flexible event timeline for the job detail screen."));

            entity.Property(e => e.JobTimelineEventId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Job).WithMany(p => p.JobTimelineEvents).HasConstraintName("fk_job_timeline_events_job");

            entity.HasOne(d => d.OccurredByUser).WithMany(p => p.JobTimelineEventoccurredByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_timeline_events_occurred_by");

            entity.HasOne(d => d.RelatedAssignment).WithMany(p => p.JobTimelineEvents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_timeline_events_assignment");

            entity.HasOne(d => d.RelatedNotary).WithMany(p => p.JobTimelineEvents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_timeline_events_related_notary");

            entity.HasOne(d => d.RelatedUser).WithMany(p => p.JobTimelineEventrelatedUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_job_timeline_events_related_user");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JobTimelineEvents)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_job_timeline_events_tenant");
        });

        modelBuilder.Entity<JournalAuditLog>(entity =>
        {
            entity.HasKey(e => e.JournalAuditLogId).HasName("journal_audit_logs_pkey");

            entity.ToTable("journal_audit_logs", "journal", tb => tb.HasComment("Evidence-grade audit trail for journal operations."));

            entity.Property(e => e.JournalAuditLogId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ActorNotary).WithMany(p => p.JournalAuditLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_audit_logs_actor_notary");

            entity.HasOne(d => d.ActorUser).WithMany(p => p.JournalAuditLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_audit_logs_actor_user");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.JournalAuditLogs).HasConstraintName("fk_journal_audit_logs_entry");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JournalAuditLogs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_audit_logs_tenant");
        });

        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasKey(e => e.JournalEntryId).HasName("journal_entries_pkey");

            entity.ToTable("journal_entries", "journal", tb => tb.HasComment("Central journal entry for each notarial act; locked entries become immutable."));

            entity.Property(e => e.JournalEntryId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ComplianceFlags).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.CurrencyCode)
                .HasDefaultValueSql("'USD'::bpchar")
                .IsFixedLength();
            entity.Property(e => e.EntryTimestamp).HasDefaultValueSql("now()");
            entity.Property(e => e.FieldSourceSummary).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.IsComplete).HasDefaultValue(false);
            entity.Property(e => e.IsLocked).HasDefaultValue(false);
            entity.Property(e => e.IsMissingSignature).HasDefaultValue(false);
            entity.Property(e => e.IsMissingThumbprint).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.SignerCount).HasDefaultValue(1);
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VenueCountryCode).IsFixedLength();
            entity.Property(e => e.VenueStateCode).IsFixedLength();

            entity.HasOne(d => d.Act).WithOne(p => p.JournalEntryact)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_entries_act");

            entity.HasOne(d => d.Branch).WithMany(p => p.JournalEntries)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_entries_branch");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.JournalEntrycreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_entries_created_by_user");

            entity.HasOne(d => d.LinkedCertificate).WithMany(p => p.JournalEntries)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_entries_linked_certificate");

            entity.HasOne(d => d.LinkedNotarialAct).WithMany(p => p.JournalEntrylinkedNotarialActs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_entries_linked_act");

            entity.HasOne(d => d.LockedByUser).WithMany(p => p.JournalEntrylockedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_entries_locked_by_user");

            entity.HasOne(d => d.Notary).WithMany(p => p.JournalEntries)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_entries_notary");

            entity.HasOne(d => d.Region).WithMany(p => p.JournalEntries)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_entries_region");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JournalEntries)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_entries_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.JournalEntryupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_entries_updated_by_user");

            entity.HasOne(d => d.VoidedByUser).WithMany(p => p.JournalEntryvoidedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_entries_voided_by_user");
        });

        modelBuilder.Entity<JournalEntryIdDocument>(entity =>
        {
            entity.HasKey(e => e.JournalEntryIdDocumentId).HasName("journal_entry_id_documents_pkey");

            entity.ToTable("journal_entry_id_documents", "journal", tb => tb.HasComment("ID scans and identity evidence for journal signers."));

            entity.Property(e => e.JournalEntryIdDocumentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CapturedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.ChecksumSha256).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StorageProvider).HasDefaultValueSql("'object_storage'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VisibilityLevel).HasDefaultValueSql("'restricted'::character varying");

            entity.HasOne(d => d.CapturedByUser).WithMany(p => p.JournalEntryIdDocuments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_entry_id_documents_captured_by");

            entity.HasOne(d => d.JournalEntrySigner).WithMany(p => p.JournalEntryIdDocuments).HasConstraintName("fk_journal_entry_id_documents_signer");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JournalEntryIdDocuments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_entry_id_documents_tenant");
        });

        modelBuilder.Entity<JournalEntryLink>(entity =>
        {
            entity.HasKey(e => e.JournalEntryLinkId).HasName("journal_entry_links_pkey");

            entity.ToTable("journal_entry_links", "journal", tb => tb.HasComment("Optional generic links to related entities."));

            entity.Property(e => e.JournalEntryLinkId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.JournalEntryLinks).HasConstraintName("fk_journal_entry_links_entry");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JournalEntryLinks)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_entry_links_tenant");
        });

        modelBuilder.Entity<JournalEntrySignature>(entity =>
        {
            entity.HasKey(e => e.JournalEntrySignatureId).HasName("journal_entry_signatures_pkey");

            entity.ToTable("journal_entry_signatures", "journal", tb => tb.HasComment("Signature capture records for journal signers."));

            entity.Property(e => e.JournalEntrySignatureId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ChecksumSha256).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.DeviceInfo).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.SignatureFormat).HasDefaultValueSql("'electronic'::character varying");
            entity.Property(e => e.StorageProvider).HasDefaultValueSql("'object_storage'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CapturedByUser).WithMany(p => p.JournalEntrySignatures)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_entry_signatures_captured_by");

            entity.HasOne(d => d.JournalEntrySigner).WithMany(p => p.JournalEntrySignatures).HasConstraintName("fk_journal_entry_signatures_signer");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JournalEntrySignatures)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_entry_signatures_tenant");
        });

        modelBuilder.Entity<JournalEntrySigner>(entity =>
        {
            entity.HasKey(e => e.JournalEntrySignerId).HasName("journal_entry_signers_pkey");

            entity.ToTable("journal_entry_signers", "journal", tb => tb.HasComment("Signer and identity information for each journal entry."));

            entity.HasIndex(e => e.JournalEntryId, "ux_journal_entry_one_primary_signer")
                .IsUnique()
                .HasFilter("((is_primary_signer = true) AND (deleted_at IS NULL))");

            entity.Property(e => e.JournalEntrySignerId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CountryCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsMissingSignature).HasDefaultValue(false);
            entity.Property(e => e.IsMissingThumbprint).HasDefaultValue(false);
            entity.Property(e => e.IsPrimarySigner).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.SignerOrder).HasDefaultValue(1);
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VerificationResult).HasDefaultValueSql("'pending'::character varying");

            entity.HasOne(d => d.JournalEntry).WithOne(p => p.JournalEntrySigner).HasConstraintName("fk_journal_entry_signers_entry");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JournalEntrySigners)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_entry_signers_tenant");
        });

        modelBuilder.Entity<JournalEntryThumbprint>(entity =>
        {
            entity.HasKey(e => e.JournalEntryThumbprintId).HasName("journal_entry_thumbprints_pkey");

            entity.ToTable("journal_entry_thumbprints", "journal", tb => tb.HasComment("Thumbprint capture records for journal signers."));

            entity.Property(e => e.JournalEntryThumbprintId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ChecksumSha256).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.DeviceInfo).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StorageProvider).HasDefaultValueSql("'object_storage'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CapturedByUser).WithMany(p => p.JournalEntryThumbprints)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_entry_thumbprints_captured_by");

            entity.HasOne(d => d.JournalEntrySigner).WithMany(p => p.JournalEntryThumbprints).HasConstraintName("fk_journal_entry_thumbprints_signer");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JournalEntryThumbprints)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_entry_thumbprints_tenant");
        });

        modelBuilder.Entity<JournalExport>(entity =>
        {
            entity.HasKey(e => e.JournalExportId).HasName("journal_exports_pkey");

            entity.ToTable("journal_exports", "journal", tb => tb.HasComment("Export requests and generated evidence packages."));

            entity.Property(e => e.JournalExportId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ChecksumSha256).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.ExportScope).HasDefaultValueSql("'single_entry'::character varying");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.RequestedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.StorageProvider).HasDefaultValueSql("'object_storage'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.GeneratedByUser).WithMany(p => p.JournalExportgeneratedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_exports_generated_by");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.JournalExports)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_exports_entry");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.JournalExportrequestedByUsers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_exports_requested_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JournalExports)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_exports_tenant");
        });

        modelBuilder.Entity<JournalRetentionPolicy>(entity =>
        {
            entity.HasKey(e => e.RetentionPolicyId).HasName("journal_retention_policies_pkey");

            entity.ToTable("journal_retention_policies", "journal", tb => tb.HasComment("State-based retention rules for journal entries."));

            entity.Property(e => e.RetentionPolicyId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.ExportAllowed).HasDefaultValue(true);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsLegalHoldEligible).HasDefaultValue(true);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.TransferAllowed).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.JournalRetentionPolicycreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_retention_policies_created_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JournalRetentionPolicies)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_retention_policies_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.JournalRetentionPolicyupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_retention_policies_updated_by");
        });

        modelBuilder.Entity<JournalTransferLog>(entity =>
        {
            entity.HasKey(e => e.JournalTransferLogId).HasName("journal_transfer_logs_pkey");

            entity.ToTable("journal_transfer_logs", "journal", tb => tb.HasComment("Transfer logs for journal custody and regulator handoff."));

            entity.Property(e => e.JournalTransferLogId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.EffectiveAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.TransferStatus).HasDefaultValueSql("'pending'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.JournalTransferLogapprovedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_transfer_logs_approved_by");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.JournalTransferLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_journal_transfer_logs_entry");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.JournalTransferLogrequestedByUsers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_transfer_logs_requested_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.JournalTransferLogs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_journal_transfer_logs_tenant");
        });

        modelBuilder.Entity<LegalHold>(entity =>
        {
            entity.HasKey(e => e.LegalHoldId).HasName("legal_holds_pkey");

            entity.ToTable("legal_holds", "compliance", tb => tb.HasComment("Legal hold records for preservation and blocking."));

            entity.Property(e => e.LegalHoldId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.AppliedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Act).WithMany(p => p.LegalHolds)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_legal_holds_act");

            entity.HasOne(d => d.AppliedByUser).WithMany(p => p.LegalHoldappliedByUsers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_legal_holds_applied_by");

            entity.HasOne(d => d.Branch).WithMany(p => p.LegalHolds)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_legal_holds_branch");

            entity.HasOne(d => d.Certificate).WithMany(p => p.LegalHolds)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_legal_holds_certificate");

            entity.HasOne(d => d.Customer).WithMany(p => p.LegalHolds)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_legal_holds_customer");

            entity.HasOne(d => d.Incident).WithMany(p => p.LegalHolds)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_legal_holds_incident");

            entity.HasOne(d => d.Job).WithMany(p => p.LegalHolds)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_legal_holds_job");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.LegalHolds)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_legal_holds_journal_entry");

            entity.HasOne(d => d.Region).WithMany(p => p.LegalHolds)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_legal_holds_region");

            entity.HasOne(d => d.ReleasedByUser).WithMany(p => p.LegalHoldreleasedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_legal_holds_released_by");

            entity.HasOne(d => d.Seal).WithMany(p => p.LegalHolds)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_legal_holds_seal");

            entity.HasOne(d => d.Tenant).WithMany(p => p.LegalHolds)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_legal_holds_tenant");
        });

        modelBuilder.Entity<MfaDevice>(entity =>
        {
            entity.HasKey(e => e.MfaDeviceId).HasName("mfa_devices_pkey");

            entity.ToTable("mfa_devices", "security", tb => tb.HasComment("MFA method registry for user authentication."));

            entity.HasIndex(e => e.UserId, "ux_mfa_devices_one_primary")
                .IsUnique()
                .HasFilter("((is_primary = true) AND (deleted_at IS NULL))");

            entity.Property(e => e.MfaDeviceId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.IsVerified).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Tenant).WithMany(p => p.MfaDevices)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_mfa_devices_tenant");

            entity.HasOne(d => d.User).WithOne(p => p.MfaDevice).HasConstraintName("fk_mfa_devices_user");
        });

        modelBuilder.Entity<NotarialAct>(entity =>
        {
            entity.HasKey(e => e.ActId).HasName("notarial_acts_pkey");

            entity.ToTable("notarial_acts", "notarial", tb => tb.HasComment("Central legal transaction record for each notarial act."));

            entity.Property(e => e.ActId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IdentityVerificationRequired).HasDefaultValue(true);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OathAdministered).HasDefaultValue(false);
            entity.Property(e => e.OathRequired).HasDefaultValue(false);
            entity.Property(e => e.PersonalAppearanceConfirmed).HasDefaultValue(false);
            entity.Property(e => e.SignerCount).HasDefaultValue(1);
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.ThumbprintRequired).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VenueCountryCode).IsFixedLength();
            entity.Property(e => e.VenueStateCode).IsFixedLength();

            entity.HasOne(d => d.ActLockedByUser).WithMany(p => p.NotarialActactLockedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notarial_acts_locked_by_user");

            entity.HasOne(d => d.ActVoidedByUser).WithMany(p => p.NotarialActactVoidedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notarial_acts_voided_by_user");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.NotarialActcreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notarial_acts_created_by_user");

            entity.HasOne(d => d.CustomerContact).WithMany(p => p.NotarialActs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notarial_acts_customer_contact");

            entity.HasOne(d => d.Customer).WithMany(p => p.NotarialActs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notarial_acts_customer");

            entity.HasOne(d => d.Job).WithMany(p => p.NotarialActs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notarial_acts_job");

            entity.HasOne(d => d.JobRequest).WithMany(p => p.NotarialActs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notarial_acts_job_request");

            entity.HasOne(d => d.LinkedCertificate).WithMany(p => p.NotarialActs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notarial_acts_linked_certificate");

            entity.HasOne(d => d.LinkedJournalEntry).WithMany(p => p.NotarialActs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notarial_acts_linked_journal_entry");

            entity.HasOne(d => d.Notary).WithMany(p => p.NotarialActs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notarial_acts_notary");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotarialActs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notarial_acts_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.NotarialActupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notarial_acts_updated_by_user");
        });

        modelBuilder.Entity<NotarialCertificate>(entity =>
        {
            entity.HasKey(e => e.CertificateId).HasName("notarial_certificates_pkey");

            entity.ToTable("notarial_certificates", "notarial", tb => tb.HasComment("Generated notarial certificate content and lock state."));

            entity.Property(e => e.CertificateId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Act).WithMany(p => p.NotarialCertificates).HasConstraintName("fk_notarial_certificates_act");

            entity.HasOne(d => d.FinalizedByUser).WithMany(p => p.NotarialCertificatefinalizedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notarial_certificates_finalized_by");

            entity.HasOne(d => d.GeneratedByUser).WithMany(p => p.NotarialCertificategeneratedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notarial_certificates_generated_by");

            entity.HasOne(d => d.LockedByUser).WithMany(p => p.NotarialCertificatelockedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notarial_certificates_locked_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotarialCertificates)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notarial_certificates_tenant");
        });

        modelBuilder.Entity<Notary>(entity =>
        {
            entity.HasKey(e => e.NotaryId).HasName("notaries_pkey");

            entity.ToTable("notaries", "identity", tb => tb.HasComment("Central notary profile linked to application user."));

            entity.Property(e => e.NotaryId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CommissioningStateCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Settings).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.TotalJobsCompleted).HasDefaultValue(0);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Branch).WithMany(p => p.Notaries)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notaries_branch");

            entity.HasOne(d => d.Region).WithMany(p => p.Notaries)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notaries_region");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Notaries)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notaries_tenant");

            entity.HasOne(d => d.User).WithOne(p => p.Notary)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notaries_user");
        });

        modelBuilder.Entity<NotaryAuditNote>(entity =>
        {
            entity.HasKey(e => e.AuditNoteId).HasName("notary_audit_notes_pkey");

            entity.ToTable("notary_audit_notes", "identity", tb => tb.HasComment("Human-readable notes for senior admins and compliance."));

            entity.Property(e => e.AuditNoteId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VisibilityLevel).HasDefaultValueSql("'restricted'::character varying");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.NotaryAuditNotecreatedByUsers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_audit_notes_created_by");

            entity.HasOne(d => d.Notary).WithMany(p => p.NotaryAuditNotes).HasConstraintName("fk_audit_notes_notary");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotaryAuditNotes)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_audit_notes_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.NotaryAuditNoteupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_audit_notes_updated_by");
        });

        modelBuilder.Entity<NotaryAvailabilityRule>(entity =>
        {
            entity.HasKey(e => e.AvailabilityRuleId).HasName("notary_availability_rules_pkey");

            entity.ToTable("notary_availability_rules", "operations", tb => tb.HasComment("Recurring availability and blackout rules for notaries."));

            entity.Property(e => e.AvailabilityRuleId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Branch).WithMany(p => p.NotaryAvailabilityRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_availability_rules_branch");

            entity.HasOne(d => d.Notary).WithMany(p => p.NotaryAvailabilityRules).HasConstraintName("fk_availability_rules_notary");

            entity.HasOne(d => d.Region).WithMany(p => p.NotaryAvailabilityRules)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_availability_rules_region");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotaryAvailabilityRules)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_availability_rules_tenant");
        });

        modelBuilder.Entity<NotaryBond>(entity =>
        {
            entity.HasKey(e => e.BondId).HasName("notary_bonds_pkey");

            entity.ToTable("notary_bonds", "identity", tb => tb.HasComment("Surety bond records."));

            entity.Property(e => e.BondId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Notary).WithMany(p => p.NotaryBonds).HasConstraintName("fk_bonds_notary");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotaryBonds)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_bonds_tenant");
        });

        modelBuilder.Entity<NotaryCapability>(entity =>
        {
            entity.HasKey(e => e.NotaryCapabilityId).HasName("notary_capabilities_pkey");

            entity.ToTable("notary_capabilities", "identity", tb => tb.HasComment("Notary service capabilities and jurisdiction scope."));

            entity.Property(e => e.NotaryCapabilityId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.AuthorizedStateCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsAuthorized).HasDefaultValue(true);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Notary).WithMany(p => p.NotaryCapabilities).HasConstraintName("fk_capabilities_notary");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotaryCapabilities)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_capabilities_tenant");
        });

        modelBuilder.Entity<NotaryCommission>(entity =>
        {
            entity.HasKey(e => e.CommissionId).HasName("notary_commissions_pkey");

            entity.ToTable("notary_commissions", "identity", tb => tb.HasComment("Commission history and current commission state."));

            entity.Property(e => e.CommissionId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CommissioningStateCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.RenewalSubmitted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Notary).WithMany(p => p.NotaryCommissions).HasConstraintName("fk_commissions_notary");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotaryCommissions)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_commissions_tenant");
        });

        modelBuilder.Entity<NotaryCommissionsPayable>(entity =>
        {
            entity.HasKey(e => e.PayableId).HasName("notary_commissions_payable_pkey");

            entity.ToTable("notary_commissions_payable", "billing", tb => tb.HasComment("Commission payable records for notaries."));

            entity.Property(e => e.PayableId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.AccrualDate).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.CurrencyCode)
                .HasDefaultValueSql("'USD'::bpchar")
                .IsFixedLength();
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.NotaryCommissionsPayableapprovedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payables_approved_by");

            entity.HasOne(d => d.Invoice).WithMany(p => p.NotaryCommissionsPayables)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payables_invoice");

            entity.HasOne(d => d.Notary).WithMany(p => p.NotaryCommissionsPayables)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_payables_notary");

            entity.HasOne(d => d.PaidByUser).WithMany(p => p.NotaryCommissionsPayablepaidByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payables_paid_by");

            entity.HasOne(d => d.Payment).WithMany(p => p.NotaryCommissionsPayables)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payables_payment");

            entity.HasOne(d => d.RevenueShare).WithMany(p => p.NotaryCommissionsPayables)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payables_revenue_share");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotaryCommissionsPayables)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_payables_tenant");
        });

        modelBuilder.Entity<NotaryDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("notary_documents_pkey");

            entity.ToTable("notary_documents", "identity", tb => tb.HasComment("Stored notary-related documents and files."));

            entity.Property(e => e.DocumentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ChecksumSha256).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsSensitive).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StorageProvider).HasDefaultValueSql("'object_storage'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VisibilityLevel).HasDefaultValueSql("'restricted'::character varying");

            entity.HasOne(d => d.Notary).WithMany(p => p.NotaryDocuments).HasConstraintName("fk_documents_notary");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotaryDocuments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_documents_tenant");

            entity.HasOne(d => d.UploadedByUser).WithMany(p => p.NotaryDocumentuploadedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_documents_uploaded_by");

            entity.HasOne(d => d.VerifiedByUser).WithMany(p => p.NotaryDocumentverifiedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_documents_verified_by");
        });

        modelBuilder.Entity<NotaryInsurance>(entity =>
        {
            entity.HasKey(e => e.InsuranceId).HasName("notary_insurances_pkey");

            entity.ToTable("notary_insurances", "identity", tb => tb.HasComment("Errors and Omissions insurance records."));

            entity.Property(e => e.InsuranceId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Notary).WithMany(p => p.NotaryInsurances).HasConstraintName("fk_insurances_notary");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotaryInsurances)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_insurances_tenant");
        });

        modelBuilder.Entity<NotaryLicense>(entity =>
        {
            entity.HasKey(e => e.LicenseId).HasName("notary_licenses_pkey");

            entity.ToTable("notary_licenses", "identity", tb => tb.HasComment("License, certification, and verification records."));

            entity.Property(e => e.LicenseId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VerificationStatus).HasDefaultValueSql("'pending'::character varying");

            entity.HasOne(d => d.Notary).WithMany(p => p.NotaryLicenses).HasConstraintName("fk_licenses_notary");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotaryLicenses)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_licenses_tenant");

            entity.HasOne(d => d.VerifiedByUser).WithMany(p => p.NotaryLicenses)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_licenses_verified_by");
        });

        modelBuilder.Entity<NotaryShiftRequest>(entity =>
        {
            entity.HasKey(e => e.ShiftRequestId).HasName("notary_shift_requests_pkey");

            entity.ToTable("notary_shift_requests", "operations", tb => tb.HasComment("Time-off, shift change, and blackout requests."));

            entity.Property(e => e.ShiftRequestId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.NotaryShiftRequestapprovedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_shift_requests_approved_by");

            entity.HasOne(d => d.Branch).WithMany(p => p.NotaryShiftRequests)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_shift_requests_branch");

            entity.HasOne(d => d.Notary).WithMany(p => p.NotaryShiftRequests).HasConstraintName("fk_shift_requests_notary");

            entity.HasOne(d => d.Region).WithMany(p => p.NotaryShiftRequests)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_shift_requests_region");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.NotaryShiftRequestrequestedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_shift_requests_requested_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotaryShiftRequests)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_shift_requests_tenant");
        });

        modelBuilder.Entity<NotaryStatusHistory>(entity =>
        {
            entity.HasKey(e => e.StatusHistoryId).HasName("notary_status_history_pkey");

            entity.ToTable("notary_status_history", "identity", tb => tb.HasComment("Status transition history for compliance and audit."));

            entity.Property(e => e.StatusHistoryId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.EffectiveAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");

            entity.HasOne(d => d.Notary).WithMany(p => p.NotaryStatusHistories).HasConstraintName("fk_status_history_notary");

            entity.HasOne(d => d.PerformedByUser).WithMany(p => p.NotaryStatusHistories)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_status_history_performed_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.NotaryStatusHistories)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_status_history_tenant");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("organizations_pkey");

            entity.ToTable("organizations", "core", tb => tb.HasComment("Company-level organizational structure."));

            entity.Property(e => e.OrganizationId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Settings).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ParentOrganization).WithMany(p => p.InverseparentOrganization)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_organizations_parent");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Organizations)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_organizations_tenant");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payments_pkey");

            entity.ToTable("payments", "billing", tb => tb.HasComment("Payment transactions and gateway references."));

            entity.Property(e => e.PaymentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.CurrencyCode)
                .HasDefaultValueSql("'USD'::bpchar")
                .IsFixedLength();
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.PaymentTime).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PaymentcreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payments_created_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_payments_customer");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payments_invoice");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payments_payment_method");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_payments_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.PaymentupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payments_updated_by");
        });

        modelBuilder.Entity<PaymentAllocation>(entity =>
        {
            entity.HasKey(e => e.PaymentAllocationId).HasName("payment_allocations_pkey");

            entity.ToTable("payment_allocations", "billing", tb => tb.HasComment("Allocations of payments to invoices."));

            entity.Property(e => e.PaymentAllocationId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.AppliedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Invoice).WithMany(p => p.PaymentAllocations).HasConstraintName("fk_payment_allocations_invoice");

            entity.HasOne(d => d.Payment).WithMany(p => p.PaymentAllocations).HasConstraintName("fk_payment_allocations_payment");

            entity.HasOne(d => d.Tenant).WithMany(p => p.PaymentAllocations)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_payment_allocations_tenant");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("payment_methods_pkey");

            entity.ToTable("payment_methods", "billing", tb => tb.HasComment("Stored customer payment methods."));

            entity.HasIndex(e => e.CustomerId, "ux_payment_methods_one_default")
                .IsUnique()
                .HasFilter("((is_default = true) AND (deleted_at IS NULL))");

            entity.Property(e => e.PaymentMethodId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.IsVerified).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CustomerContact).WithMany(p => p.PaymentMethods)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payment_methods_customer_contact");

            entity.HasOne(d => d.Customer).WithOne(p => p.PaymentMethod).HasConstraintName("fk_payment_methods_customer");

            entity.HasOne(d => d.Tenant).WithMany(p => p.PaymentMethods)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_payment_methods_tenant");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("permissions_pkey");

            entity.ToTable("permissions", "core", tb => tb.HasComment("Global permission catalog."));

            entity.Property(e => e.PermissionId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<PolicyVersion>(entity =>
        {
            entity.HasKey(e => e.PolicyVersionId).HasName("policy_versions_pkey");

            entity.ToTable("policy_versions", "compliance", tb => tb.HasComment("Versioned compliance policies and rule bundles."));

            entity.Property(e => e.PolicyVersionId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.PolicyJson).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.VersionNo).HasDefaultValue(1);

            entity.HasOne(d => d.Branch).WithMany(p => p.PolicyVersions)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_policy_versions_branch");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PolicyVersioncreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_policy_versions_created_by");

            entity.HasOne(d => d.ParentPolicyVersion).WithMany(p => p.InverseparentPolicyVersion)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_policy_versions_parent");

            entity.HasOne(d => d.Region).WithMany(p => p.PolicyVersions)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_policy_versions_region");

            entity.HasOne(d => d.Tenant).WithMany(p => p.PolicyVersions)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_policy_versions_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.PolicyVersionupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_policy_versions_updated_by");
        });

        modelBuilder.Entity<PricingPlan>(entity =>
        {
            entity.HasKey(e => e.PricingPlanId).HasName("pricing_plans_pkey");

            entity.ToTable("pricing_plans", "crm", tb => tb.HasComment("Commercial pricing plan linked to a customer."));

            entity.Property(e => e.PricingPlanId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.CurrencyCode)
                .HasDefaultValueSql("'USD'::bpchar")
                .IsFixedLength();
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PricingPlans)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_pricing_plans_created_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.PricingPlans).HasConstraintName("fk_pricing_plans_customer");

            entity.HasOne(d => d.Tenant).WithMany(p => p.PricingPlans)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_pricing_plans_tenant");
        });

        modelBuilder.Entity<PricingRule>(entity =>
        {
            entity.HasKey(e => e.PricingRuleId).HasName("pricing_rules_pkey");

            entity.ToTable("pricing_rules", "crm", tb => tb.HasComment("Detailed pricing rules and tiers."));

            entity.Property(e => e.PricingRuleId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.PricingPlan).WithMany(p => p.PricingRules).HasConstraintName("fk_pricing_rules_plan");

            entity.HasOne(d => d.Tenant).WithMany(p => p.PricingRules)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_pricing_rules_tenant");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId).HasName("refresh_tokens_pkey");

            entity.ToTable("refresh_tokens", "core", tb => tb.HasComment("Refresh token/session tracking for authentication."));

            entity.Property(e => e.RefreshTokenId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.TokenHash).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ReplacedByToken).WithMany(p => p.InverseReplacedByToken)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_refresh_tokens_replaced_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.RefreshTokens)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_refresh_tokens_tenant");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens).HasConstraintName("fk_refresh_tokens_user");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.RefundId).HasName("refunds_pkey");

            entity.ToTable("refunds", "billing", tb => tb.HasComment("Refund transactions and statuses."));

            entity.Property(e => e.RefundId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.CurrencyCode)
                .HasDefaultValueSql("'USD'::bpchar")
                .IsFixedLength();
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.RefundDate).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.RefundcreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_refunds_created_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.Refunds)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_refunds_customer");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Refunds)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_refunds_invoice");

            entity.HasOne(d => d.Payment).WithMany(p => p.Refunds)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_refunds_payment");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Refunds)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_refunds_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.RefundupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_refunds_updated_by");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("regions_pkey");

            entity.ToTable("regions", "core", tb => tb.HasComment("Service regions used for dispatch and compliance."));

            entity.Property(e => e.RegionId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CountryCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Settings).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Organization).WithMany(p => p.Regions)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_regions_organization");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Regions)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_regions_tenant");
        });

        modelBuilder.Entity<RegulatoryExport>(entity =>
        {
            entity.HasKey(e => e.RegulatoryExportId).HasName("regulatory_exports_pkey");

            entity.ToTable("regulatory_exports", "compliance", tb => tb.HasComment("Exports prepared for regulators or audits."));

            entity.Property(e => e.RegulatoryExportId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ChecksumSha256).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.RequestedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.StorageProvider).HasDefaultValueSql("'object_storage'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.GeneratedByUser).WithMany(p => p.RegulatoryExportgeneratedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_regulatory_exports_generated_by");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.RegulatoryExportrequestedByUsers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_regulatory_exports_requested_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.RegulatoryExports)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_regulatory_exports_tenant");
        });

        modelBuilder.Entity<RetentionJob>(entity =>
        {
            entity.HasKey(e => e.RetentionJobId).HasName("retention_jobs_pkey");

            entity.ToTable("retention_jobs", "compliance", tb => tb.HasComment("Scheduled/executed retention jobs."));

            entity.Property(e => e.RetentionJobId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.JobStatus).HasDefaultValueSql("'pending'::character varying");
            entity.Property(e => e.LegalHoldBlocked).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.ScheduledAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ExecutedByUser).WithMany(p => p.RetentionJobexecutedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_retention_jobs_executed_by");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.RetentionJobrequestedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_retention_jobs_requested_by");

            entity.HasOne(d => d.RetentionPolicy).WithMany(p => p.RetentionJobs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_retention_jobs_policy");

            entity.HasOne(d => d.Tenant).WithMany(p => p.RetentionJobs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_retention_jobs_tenant");
        });

        modelBuilder.Entity<RetentionPolicy>(entity =>
        {
            entity.HasKey(e => e.RetentionPolicyId).HasName("retention_policies_pkey");

            entity.ToTable("retention_policies", "compliance", tb => tb.HasComment("Retention rules for records and artifacts."));

            entity.Property(e => e.RetentionPolicyId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.DestroyAfterRetention).HasDefaultValue(false);
            entity.Property(e => e.LegalHoldEligible).HasDefaultValue(true);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.RetentionPolicycreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_retention_policies_created_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.RetentionPolicies)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_retention_policies_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.RetentionPolicyupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_retention_policies_updated_by");
        });

        modelBuilder.Entity<RevenueShare>(entity =>
        {
            entity.HasKey(e => e.RevenueShareId).HasName("revenue_shares_pkey");

            entity.ToTable("revenue_shares", "billing", tb => tb.HasComment("Revenue share calculations for jobs, acts, invoices, and notaries."));

            entity.Property(e => e.RevenueShareId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Contract).WithMany(p => p.RevenueShares)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_revenue_shares_contract");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.RevenueSharecreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_revenue_shares_created_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.RevenueShares)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_revenue_shares_customer");

            entity.HasOne(d => d.Invoice).WithMany(p => p.RevenueShares)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_revenue_shares_invoice");

            entity.HasOne(d => d.Job).WithMany(p => p.RevenueShares)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_revenue_shares_job");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.RevenueShares)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_revenue_shares_journal_entry");

            entity.HasOne(d => d.NotarialAct).WithMany(p => p.RevenueShares)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_revenue_shares_notarial_act");

            entity.HasOne(d => d.Notary).WithMany(p => p.RevenueShares)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_revenue_shares_notary");

            entity.HasOne(d => d.Tenant).WithMany(p => p.RevenueShares)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_revenue_shares_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.RevenueShareupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_revenue_shares_updated_by");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.ToTable("roles", "core", tb => tb.HasComment("Tenant-scoped roles."));

            entity.Property(e => e.RoleId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSystem).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Roles)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_roles_tenant");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { role_id = e.RoleId, permission_id = e.PermissionId }).HasName("role_permissions_pkey");

            entity.ToTable("role_permissions", "core", tb => tb.HasComment("Many-to-many mapping between roles and permissions."));

            entity.Property(e => e.GrantedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions).HasConstraintName("fk_role_permissions_permission");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions).HasConstraintName("fk_role_permissions_role");
        });

        modelBuilder.Entity<ScheduleBlock>(entity =>
        {
            entity.HasKey(e => e.ScheduleBlockId).HasName("schedule_blocks_pkey");

            entity.ToTable("schedule_blocks", "operations", tb => tb.HasComment("Master scheduling board entries and calendar blocks."));

            entity.Property(e => e.ScheduleBlockId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CountryCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsAllDay).HasDefaultValue(false);
            entity.Property(e => e.IsConflict).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.Timezone).HasDefaultValueSql("'UTC'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Branch).WithMany(p => p.ScheduleBlocks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_schedule_blocks_branch");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ScheduleBlockcreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_schedule_blocks_created_by");

            entity.HasOne(d => d.JobAssignment).WithMany(p => p.ScheduleBlocks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_schedule_blocks_job_assignment");

            entity.HasOne(d => d.Job).WithMany(p => p.ScheduleBlocks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_schedule_blocks_job");

            entity.HasOne(d => d.Notary).WithMany(p => p.ScheduleBlocks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_schedule_blocks_notary");

            entity.HasOne(d => d.Region).WithMany(p => p.ScheduleBlocks)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_schedule_blocks_region");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ScheduleBlocks)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_schedule_blocks_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.ScheduleBlockupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_schedule_blocks_updated_by");
        });

        modelBuilder.Entity<Seal>(entity =>
        {
            entity.HasKey(e => e.SealId).HasName("seals_pkey");

            entity.ToTable("seals", "security", tb => tb.HasComment("Physical seal and eSeal inventory for each notary."));

            entity.Property(e => e.SealId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.ImageChecksumSha256).IsFixedLength();
            entity.Property(e => e.ImageStorageProvider).HasDefaultValueSql("'object_storage'::character varying");
            entity.Property(e => e.IsAvailableForUse).HasDefaultValue(true);
            entity.Property(e => e.IsCentralized).HasDefaultValue(true);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UsageCount).HasDefaultValue(0);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.SealcreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_seals_created_by");

            entity.HasOne(d => d.Notary).WithMany(p => p.Seals)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_seals_notary");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Seals)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_seals_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.SealupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_seals_updated_by");
        });

        modelBuilder.Entity<SealAccessPolicy>(entity =>
        {
            entity.HasKey(e => e.PolicyId).HasName("seal_access_policies_pkey");

            entity.ToTable("seal_access_policies", "security", tb => tb.HasComment("Who may use a seal or certificate, under what conditions, and with what approvals."));

            entity.Property(e => e.PolicyId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ApprovalRequired).HasDefaultValue(false);
            entity.Property(e => e.ApprovalWorkflow).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.Conditions).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.DelegationAllowed).HasDefaultValue(false);
            entity.Property(e => e.EmergencyOverrideAllowed).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.MfaRequired).HasDefaultValue(true);
            entity.Property(e => e.RequiredPermissions).HasDefaultValueSql("'[]'::jsonb");
            entity.Property(e => e.RequiredRoles).HasDefaultValueSql("'[]'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Branch).WithMany(p => p.SealAccessPolicies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_policies_branch");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.SealAccessPolicycreatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_policies_created_by");

            entity.HasOne(d => d.Notary).WithMany(p => p.SealAccessPolicies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_policies_notary");

            entity.HasOne(d => d.Region).WithMany(p => p.SealAccessPolicies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_policies_region");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.SealAccessPolicies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_policies_service_type");

            entity.HasOne(d => d.Tenant).WithMany(p => p.SealAccessPolicies)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_policies_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.SealAccessPolicyupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_policies_updated_by");
        });

        modelBuilder.Entity<SealReplacement>(entity =>
        {
            entity.HasKey(e => e.ReplacementId).HasName("seal_replacements_pkey");

            entity.ToTable("seal_replacements", "security", tb => tb.HasComment("Replacement history for seals and certificates."));

            entity.Property(e => e.ReplacementId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.EffectiveAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IssuedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Incident).WithMany(p => p.SealReplacements)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_replacements_incident");

            entity.HasOne(d => d.IssuedByUser).WithMany(p => p.SealReplacements)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_replacements_issued_by");

            entity.HasOne(d => d.NewCertificate).WithMany(p => p.SealReplacementnewCertificates)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_replacements_new_certificate");

            entity.HasOne(d => d.NewSeal).WithMany(p => p.SealReplacementnewSeals)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_replacements_new_seal");

            entity.HasOne(d => d.OldCertificate).WithMany(p => p.SealReplacementoldCertificates)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_replacements_old_certificate");

            entity.HasOne(d => d.OldSeal).WithMany(p => p.SealReplacementoldSeals)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_replacements_old_seal");

            entity.HasOne(d => d.Revocation).WithMany(p => p.SealReplacements)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_replacements_revocation");

            entity.HasOne(d => d.Tenant).WithMany(p => p.SealReplacements)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_replacements_tenant");
        });

        modelBuilder.Entity<SealRevocation>(entity =>
        {
            entity.HasKey(e => e.RevocationId).HasName("seal_revocations_pkey");

            entity.ToTable("seal_revocations", "security", tb => tb.HasComment("Revocation history for seals and digital certificates."));

            entity.Property(e => e.RevocationId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.EffectiveAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.RegulatoryNotificationRequired).HasDefaultValue(false);
            entity.Property(e => e.RevokedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.DigitalCertificate).WithMany(p => p.SealRevocations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_revocations_certificate");

            entity.HasOne(d => d.Incident).WithMany(p => p.SealRevocations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_revocations_incident");

            entity.HasOne(d => d.RevokedByUser).WithMany(p => p.SealRevocations)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_revocations_revoked_by");

            entity.HasOne(d => d.Seal).WithMany(p => p.SealRevocations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_revocations_seal");

            entity.HasOne(d => d.Tenant).WithMany(p => p.SealRevocations)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_revocations_tenant");
        });

        modelBuilder.Entity<SealUsageLog>(entity =>
        {
            entity.HasKey(e => e.UsageLogId).HasName("seal_usage_logs_pkey");

            entity.ToTable("seal_usage_logs", "security", tb => tb.HasComment("Traceable usage logs for seal and certificate operations."));

            entity.Property(e => e.UsageLogId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Evidence).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.StateCode).IsFixedLength();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UsedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Device).WithMany(p => p.SealUsageLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_usage_logs_device");

            entity.HasOne(d => d.DigitalCertificate).WithMany(p => p.SealUsageLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_usage_logs_certificate");

            entity.HasOne(d => d.Incident).WithMany(p => p.SealUsageLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_usage_logs_incident");

            entity.HasOne(d => d.JournalEntry).WithMany(p => p.SealUsageLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_usage_logs_journal_entry");

            entity.HasOne(d => d.NotarialAct).WithMany(p => p.SealUsageLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_usage_logs_notarial_act");

            entity.HasOne(d => d.Notary).WithMany(p => p.SealUsageLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_usage_logs_notary");

            entity.HasOne(d => d.Policy).WithMany(p => p.SealUsageLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_usage_logs_policy");

            entity.HasOne(d => d.Seal).WithMany(p => p.SealUsageLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_usage_logs_seal");

            entity.HasOne(d => d.Tenant).WithMany(p => p.SealUsageLogs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_usage_logs_tenant");

            entity.HasOne(d => d.UsedByUser).WithMany(p => p.SealUsageLogs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_usage_logs_used_by");
        });

        modelBuilder.Entity<SecurityAuditLog>(entity =>
        {
            entity.HasKey(e => e.SecurityAuditLogId).HasName("security_audit_logs_pkey");

            entity.ToTable("security_audit_logs", "security", tb => tb.HasComment("Evidence-grade audit trail for security operations."));

            entity.Property(e => e.SecurityAuditLogId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.ActorNotary).WithMany(p => p.SecurityAuditLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_security_audit_logs_actor_notary");

            entity.HasOne(d => d.ActorUser).WithMany(p => p.SecurityAuditLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_security_audit_logs_actor_user");

            entity.HasOne(d => d.Tenant).WithMany(p => p.SecurityAuditLogs)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_security_audit_logs_tenant");
        });

        modelBuilder.Entity<SecurityIncident>(entity =>
        {
            entity.HasKey(e => e.IncidentId).HasName("security_incidents_pkey");

            entity.ToTable("security_incidents", "security", tb => tb.HasComment("Security incidents, compromises, unauthorized use, and response lifecycle."));

            entity.Property(e => e.IncidentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.DetectedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.LegalHold).HasDefaultValue(false);
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.RegulatoryNotificationRequired).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.AffectedCertificate).WithMany(p => p.SecurityIncidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_security_incidents_affected_certificate");

            entity.HasOne(d => d.AffectedDevice).WithMany(p => p.SecurityIncidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_security_incidents_affected_device");

            entity.HasOne(d => d.AffectedSeal).WithMany(p => p.SecurityIncidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_security_incidents_affected_seal");

            entity.HasOne(d => d.AssignedToUser).WithMany(p => p.SecurityIncidentassignedToUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_security_incidents_assigned_to");

            entity.HasOne(d => d.PrimaryNotary).WithMany(p => p.SecurityIncidents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_security_incidents_primary_notary");

            entity.HasOne(d => d.ReportedByUser).WithMany(p => p.SecurityIncidentreportedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_security_incidents_reported_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.SecurityIncidents)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_security_incidents_tenant");
        });

        modelBuilder.Entity<SecurityIncidentAction>(entity =>
        {
            entity.HasKey(e => e.IncidentActionId).HasName("security_incident_actions_pkey");

            entity.ToTable("security_incident_actions", "security", tb => tb.HasComment("Action tracking inside a security incident."));

            entity.Property(e => e.IncidentActionId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ActionStatus).HasDefaultValueSql("'open'::character varying");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.PerformedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Incident).WithMany(p => p.SecurityIncidentActions).HasConstraintName("fk_incident_actions_incident");

            entity.HasOne(d => d.PerformedByUser).WithMany(p => p.SecurityIncidentActions)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_incident_actions_performed_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.SecurityIncidentActions)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_incident_actions_tenant");
        });

        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.HasKey(e => e.ServiceTypeId).HasName("service_types_pkey");

            entity.ToTable("service_types", "operations", tb => tb.HasComment("Operational service catalog used by requests, jobs, and dispatch."));

            entity.Property(e => e.ServiceTypeId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RequiresIdentityVerification).HasDefaultValue(true);
            entity.Property(e => e.RequiresJournalEntry).HasDefaultValue(true);
            entity.Property(e => e.RequiresSeal).HasDefaultValue(true);
            entity.Property(e => e.RequiresSignature).HasDefaultValue(true);
            entity.Property(e => e.Settings).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ServiceTypes)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_service_types_tenant");
        });

        modelBuilder.Entity<SlaAgreement>(entity =>
        {
            entity.HasKey(e => e.SlaId).HasName("sla_agreements_pkey");

            entity.ToTable("sla_agreements", "crm", tb => tb.HasComment("Service-level agreements linked to contracts."));

            entity.Property(e => e.SlaId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Contract).WithMany(p => p.SlaAgreements).HasConstraintName("fk_sla_agreements_contract");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.SlaAgreementcreatedByUsers)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sla_agreements_created_by");

            entity.HasOne(d => d.Tenant).WithMany(p => p.SlaAgreements)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sla_agreements_tenant");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.SlaAgreementupdatedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_sla_agreements_updated_by");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("teams_pkey");

            entity.ToTable("teams", "core", tb => tb.HasComment("Work teams under branches/regions."));

            entity.Property(e => e.TeamId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Settings).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Branch).WithMany(p => p.Teams)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_teams_branch");

            entity.HasOne(d => d.Region).WithMany(p => p.Teams)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_teams_region");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Teams)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_teams_tenant");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.TenantId).HasName("tenants_pkey");

            entity.ToTable("tenants", "core", tb => tb.HasComment("Tenant root record for multi-tenant isolation."));

            entity.Property(e => e.TenantId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.DefaultTimezone).HasDefaultValueSql("'UTC'::character varying");
            entity.Property(e => e.PrimaryCountryCode).IsFixedLength();
            entity.Property(e => e.Settings).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<TrustedDevice>(entity =>
        {
            entity.HasKey(e => e.TrustedDeviceId).HasName("trusted_devices_pkey");

            entity.ToTable("trusted_devices", "security", tb => tb.HasComment("Trusted device registry for access control and step-up authentication."));

            entity.Property(e => e.TrustedDeviceId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.FirstSeenAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Metadata).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Tenant).WithMany(p => p.TrustedDevices)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_trusted_devices_tenant");

            entity.HasOne(d => d.User).WithMany(p => p.TrustedDevices).HasConstraintName("fk_trusted_devices_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users", "core", tb => tb.HasComment("Application users and staff accounts."));

            entity.Property(e => e.UserId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Locale).HasDefaultValueSql("'en-US'::character varying");
            entity.Property(e => e.Settings).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.TimeZone).HasDefaultValueSql("'UTC'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Branch).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_users_branch");

            entity.HasOne(d => d.Organization).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_users_organization");

            entity.HasOne(d => d.Team).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_users_team");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_users_tenant");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { user_id = e.UserId, role_id = e.RoleId }).HasName("user_roles_pkey");

            entity.ToTable("user_roles", "core", tb => tb.HasComment("Many-to-many mapping between users and roles."));

            entity.Property(e => e.AssignedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.AssignedByUser).WithMany(p => p.UserRoleassignedByUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_user_roles_assigned_by_user");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles).HasConstraintName("fk_user_roles_role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoleusers).HasConstraintName("fk_user_roles_user");
        });

        modelBuilder.Entity<VActLatestCertificate>(entity =>
        {
            entity.ToView("v_act_latest_certificate", "notarial");

            entity.Property(e => e.StateCode).IsFixedLength();
        });

        modelBuilder.Entity<VActOverview>(entity =>
        {
            entity.ToView("v_act_overview", "notarial");

            entity.Property(e => e.StateCode).IsFixedLength();
        });

        modelBuilder.Entity<VActiveRule>(entity =>
        {
            entity.ToView("v_active_rules", "compliance");

            entity.Property(e => e.StateCode).IsFixedLength();
        });

        modelBuilder.Entity<VActiveSeal>(entity =>
        {
            entity.ToView("v_active_seals", "security");

            entity.Property(e => e.StateCode).IsFixedLength();
        });

        modelBuilder.Entity<VActiveUser>(entity =>
        {
            entity.ToView("v_active_users", "core");
        });

        modelBuilder.Entity<VAgingSummary>(entity =>
        {
            entity.ToView("v_aging_summary", "billing");
        });

        modelBuilder.Entity<VCertificateInventory>(entity =>
        {
            entity.ToView("v_certificate_inventory", "security");
        });

        modelBuilder.Entity<VCurrentPolicyVersion>(entity =>
        {
            entity.ToView("v_current_policy_versions", "compliance");

            entity.Property(e => e.StateCode).IsFixedLength();
        });

        modelBuilder.Entity<VCurrentPrimaryAssignment>(entity =>
        {
            entity.ToView("v_current_primary_assignment", "operations");
        });

        modelBuilder.Entity<VCustomerOverview>(entity =>
        {
            entity.ToView("v_customer_overview", "crm");
        });

        modelBuilder.Entity<VCustomerPrimaryContact>(entity =>
        {
            entity.ToView("v_customer_primary_contact", "crm");
        });

        modelBuilder.Entity<VInvoiceOverview>(entity =>
        {
            entity.ToView("v_invoice_overview", "billing");

            entity.Property(e => e.CurrencyCode).IsFixedLength();
        });

        modelBuilder.Entity<VJobBoard>(entity =>
        {
            entity.ToView("v_job_board", "operations");
        });

        modelBuilder.Entity<VJournalComplianceSummary>(entity =>
        {
            entity.ToView("v_journal_compliance_summary", "journal");

            entity.Property(e => e.StateCode).IsFixedLength();
        });

        modelBuilder.Entity<VJournalEntryOverview>(entity =>
        {
            entity.ToView("v_journal_entry_overview", "journal");

            entity.Property(e => e.CurrencyCode).IsFixedLength();
            entity.Property(e => e.StateCode).IsFixedLength();
        });

        modelBuilder.Entity<VJournalEntrySignerOverview>(entity =>
        {
            entity.ToView("v_journal_entry_signer_overview", "journal");
        });

        modelBuilder.Entity<VLatestMessagePerThread>(entity =>
        {
            entity.ToView("v_latest_message_per_thread", "communication");
        });

        modelBuilder.Entity<VNotaryComplianceStatus>(entity =>
        {
            entity.ToView("v_notary_compliance_status", "identity");
        });

        modelBuilder.Entity<VNotaryDailySchedule>(entity =>
        {
            entity.ToView("v_notary_daily_schedule", "operations");
        });

        modelBuilder.Entity<VNotaryProfileOverview>(entity =>
        {
            entity.ToView("v_notary_profile_overview", "identity");

            entity.Property(e => e.CommissioningStateCode).IsFixedLength();
        });

        modelBuilder.Entity<VOpenIncident>(entity =>
        {
            entity.ToView("v_open_incidents", "compliance");

            entity.Property(e => e.StateCode).IsFixedLength();
        });

        modelBuilder.Entity<VPendingReminder>(entity =>
        {
            entity.ToView("v_pending_reminders", "communication");
        });

        modelBuilder.Entity<VRevenueShareSummary>(entity =>
        {
            entity.ToView("v_revenue_share_summary", "billing");
        });

        modelBuilder.Entity<VSecurityRiskSummary>(entity =>
        {
            entity.ToView("v_security_risk_summary", "security");
        });

        modelBuilder.Entity<VThreadOverview>(entity =>
        {
            entity.ToView("v_thread_overview", "communication");
        });

        modelBuilder.Entity<VoidReason>(entity =>
        {
            entity.HasKey(e => e.VoidReasonId).HasName("void_reasons_pkey");

            entity.ToTable("void_reasons", "notarial", tb => tb.HasComment("Catalog of allowed void reasons."));

            entity.Property(e => e.VoidReasonId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Tenant).WithMany(p => p.VoidReasons)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_void_reasons_tenant");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
