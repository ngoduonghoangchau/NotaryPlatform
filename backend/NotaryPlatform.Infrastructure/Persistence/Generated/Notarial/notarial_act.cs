using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;

/// <summary>
/// Central legal transaction record for each notarial act.
/// </summary>
[Table("notarial_acts", Schema = "notarial")]
[Index("act_completed_at", Name = "ix_notarial_acts_act_completed_at")]
[Index("customer_id", Name = "ix_notarial_acts_customer_id")]
[Index("job_id", Name = "ix_notarial_acts_job_id")]
[Index("notary_id", Name = "ix_notarial_acts_notary_id")]
[Index("state_code", Name = "ix_notarial_acts_state_code")]
[Index("tenant_id", Name = "ix_notarial_acts_tenant_id")]
[Index("tenant_id", "act_code", Name = "uq_notarial_acts_tenant_code", IsUnique = true)]
public partial class NotarialAct
{
    [Key]
    public Guid ActId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string ActCode { get; set; } = null!;

    public Guid? JobId { get; set; }

    public Guid? JobRequestId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? CustomerContactId { get; set; }

    public Guid NotaryId { get; set; }

    [StringLength(2)]
    public string StateCode { get; set; } = null!;

    [StringLength(200)]
    public string? VenueName { get; set; }

    [StringLength(200)]
    public string? VenueAddressLine1 { get; set; }

    [StringLength(200)]
    public string? VenueAddressLine2 { get; set; }

    [StringLength(100)]
    public string? VenueCity { get; set; }

    [StringLength(2)]
    public string? VenueStateCode { get; set; }

    [StringLength(20)]
    public string? VenuePostalCode { get; set; }

    [StringLength(2)]
    public string? VenueCountryCode { get; set; }

    [Precision(10, 7)]
    public decimal? Latitude { get; set; }

    [Precision(10, 7)]
    public decimal? Longitude { get; set; }

    [StringLength(300)]
    public string? DocumentTitle { get; set; }

    public int? DocumentPageCount { get; set; }

    [StringLength(100)]
    public string? DocumentReferenceNo { get; set; }

    public int SignerCount { get; set; }

    public bool OathRequired { get; set; }

    public bool ThumbprintRequired { get; set; }

    public bool IdentityVerificationRequired { get; set; }

    public bool PersonalAppearanceConfirmed { get; set; }

    public bool OathAdministered { get; set; }

    public DateTime? ActStartedAt { get; set; }

    public DateTime? ActCompletedAt { get; set; }

    public DateTime? ActLockedAt { get; set; }

    public Guid? ActLockedByUserId { get; set; }

    public DateTime? ActVoidedAt { get; set; }

    public Guid? ActVoidedByUserId { get; set; }

    public string? VoidReason { get; set; }

    public Guid? LinkedJournalEntryId { get; set; }

    public Guid? LinkedCertificateId { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("act")]
    public virtual ICollection<ActAuditLog> ActAuditLogs { get; set; } = new List<ActAuditLog>();

    [InverseProperty("act")]
    public virtual ICollection<ActDocument> ActDocuments { get; set; } = new List<ActDocument>();

    [InverseProperty("act")]
    public virtual ICollection<ActExecutionRecord> ActExecutionRecords { get; set; } = new List<ActExecutionRecord>();

    [InverseProperty("act")]
    public virtual ICollection<ActIdentityVerification> ActIdentityVerifications { get; set; } = new List<ActIdentityVerification>();

    [ForeignKey("act_locked_by_user_id")]
    [InverseProperty("notarial_actact_locked_by_users")]
    public virtual User? ActLockedByUser { get; set; }

    [InverseProperty("act")]
    public virtual ActSigner? ActSigner { get; set; }

    [InverseProperty("act")]
    public virtual ICollection<ActStatusHistory> ActStatusHistories { get; set; } = new List<ActStatusHistory>();

    [ForeignKey("act_voided_by_user_id")]
    [InverseProperty("notarial_actact_voided_by_users")]
    public virtual User? ActVoidedByUser { get; set; }

    [InverseProperty("notarial_act")]
    public virtual ICollection<CommunicationReminder> CommunicationReminders { get; set; } = new List<CommunicationReminder>();

    [InverseProperty("notarial_act")]
    public virtual ICollection<CommunicationThread> CommunicationThreads { get; set; } = new List<CommunicationThread>();

    [InverseProperty("act")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [InverseProperty("act")]
    public virtual ICollection<ComplianceRule> ComplianceRules { get; set; } = new List<ComplianceRule>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("notarial_actcreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("notarial_acts")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("customer_contact_id")]
    [InverseProperty("notarial_acts")]
    public virtual CustomerContact? CustomerContact { get; set; }

    [InverseProperty("act")]
    public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    [InverseProperty("notarial_act")]
    public virtual ICollection<InternalNote> InternalNotes { get; set; } = new List<InternalNote>();

    [InverseProperty("notarial_act")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    [ForeignKey("job_id")]
    [InverseProperty("notarial_acts")]
    public virtual Job? Job { get; set; }

    [ForeignKey("job_request_id")]
    [InverseProperty("notarial_acts")]
    public virtual JobRequest? JobRequest { get; set; }

    [InverseProperty("act")]
    public virtual JournalEntry? JournalEntryact { get; set; }

    [InverseProperty("linked_notarial_act")]
    public virtual ICollection<JournalEntry> JournalEntrylinkedNotarialActs { get; set; } = new List<JournalEntry>();

    [InverseProperty("act")]
    public virtual ICollection<LegalHold> LegalHolds { get; set; } = new List<LegalHold>();

    [ForeignKey("linked_certificate_id")]
    [InverseProperty("notarial_acts")]
    public virtual NotarialCertificate? LinkedCertificate { get; set; }

    [ForeignKey("linked_journal_entry_id")]
    [InverseProperty("notarial_acts")]
    public virtual JournalEntry? LinkedJournalEntry { get; set; }

    [InverseProperty("act")]
    public virtual ICollection<NotarialCertificate> NotarialCertificates { get; set; } = new List<NotarialCertificate>();

    [ForeignKey("notary_id")]
    [InverseProperty("notarial_acts")]
    public virtual Notary Notary { get; set; } = null!;

    [InverseProperty("notarial_act")]
    public virtual ICollection<RevenueShare> RevenueShares { get; set; } = new List<RevenueShare>();

    [InverseProperty("notarial_act")]
    public virtual ICollection<SealUsageLog> SealUsageLogs { get; set; } = new List<SealUsageLog>();

    [ForeignKey("tenant_id")]
    [InverseProperty("notarial_acts")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("notarial_actupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class NotarialAct
{
    public NotarialActType act_type { get; set; }
    public AppearanceType appearance_type { get; set; }
    public NotarialActStatus status { get; set; }
}
// </auto-enum-partial>
