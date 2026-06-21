using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;
using NotaryPlatform.Domain.Features.Journal.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Journal;

/// <summary>
/// Central journal entry for each notarial act; locked entries become immutable.
/// </summary>
[Table("journal_entries", Schema = "journal")]
[Index("act_id", Name = "ix_journal_entries_act_id")]
[Index("entry_timestamp", Name = "ix_journal_entries_entry_timestamp")]
[Index("notary_id", Name = "ix_journal_entries_notary_id")]
[Index("state_code", Name = "ix_journal_entries_state_code")]
[Index("tenant_id", Name = "ix_journal_entries_tenant_id")]
[Index("act_id", Name = "uq_journal_entries_act", IsUnique = true)]
[Index("tenant_id", "entry_code", Name = "uq_journal_entries_tenant_code", IsUnique = true)]
public partial class JournalEntry
{
    [Key]
    public Guid JournalEntryId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string EntryCode { get; set; } = null!;

    public Guid ActId { get; set; }

    public Guid NotaryId { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

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

    public DateOnly EntryDate { get; set; }

    public TimeOnly EntryTime { get; set; }

    public DateTime EntryTimestamp { get; set; }

    [StringLength(100)]
    public string? ActType { get; set; }

    public int SignerCount { get; set; }

    [Precision(18, 2)]
    public decimal FeeCharged { get; set; }

    [StringLength(3)]
    public string CurrencyCode { get; set; } = null!;

    [StringLength(50)]
    public string? SourceChannel { get; set; }

    public string? SourceReference { get; set; }

    [Column(TypeName = "jsonb")]
    public string FieldSourceSummary { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string ComplianceFlags { get; set; } = null!;

    public string? Notes { get; set; }

    public bool IsMissingThumbprint { get; set; }

    public bool IsMissingSignature { get; set; }

    public bool IsComplete { get; set; }

    public bool IsLocked { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? LockedAt { get; set; }

    public Guid? LockedByUserId { get; set; }

    public string? LockReason { get; set; }

    public DateTime? VoidedAt { get; set; }

    public Guid? VoidedByUserId { get; set; }

    public string? VoidReason { get; set; }

    public Guid LinkedNotarialActId { get; set; }

    public Guid? LinkedCertificateId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("act_id")]
    [InverseProperty("journal_entryact")]
    public virtual NotarialAct Act { get; set; } = null!;

    [ForeignKey("branch_id")]
    [InverseProperty("journal_entries")]
    public virtual Branch? Branch { get; set; }

    [InverseProperty("journal_entry")]
    public virtual ICollection<CommunicationReminder> CommunicationReminders { get; set; } = new List<CommunicationReminder>();

    [InverseProperty("journal_entry")]
    public virtual ICollection<CommunicationThread> CommunicationThreads { get; set; } = new List<CommunicationThread>();

    [InverseProperty("journal_entry")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [InverseProperty("journal_entry")]
    public virtual ICollection<ComplianceRule> ComplianceRules { get; set; } = new List<ComplianceRule>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("journal_entrycreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [InverseProperty("journal_entry")]
    public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    [InverseProperty("journal_entry")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    [InverseProperty("journal_entry")]
    public virtual ICollection<JournalAuditLog> JournalAuditLogs { get; set; } = new List<JournalAuditLog>();

    [InverseProperty("journal_entry")]
    public virtual ICollection<JournalEntryLink> JournalEntryLinks { get; set; } = new List<JournalEntryLink>();

    [InverseProperty("journal_entry")]
    public virtual JournalEntrySigner? JournalEntrySigner { get; set; }

    [InverseProperty("journal_entry")]
    public virtual ICollection<JournalExport> JournalExports { get; set; } = new List<JournalExport>();

    [InverseProperty("journal_entry")]
    public virtual ICollection<JournalTransferLog> JournalTransferLogs { get; set; } = new List<JournalTransferLog>();

    [InverseProperty("journal_entry")]
    public virtual ICollection<LegalHold> LegalHolds { get; set; } = new List<LegalHold>();

    [ForeignKey("linked_certificate_id")]
    [InverseProperty("journal_entries")]
    public virtual NotarialCertificate? LinkedCertificate { get; set; }

    [ForeignKey("linked_notarial_act_id")]
    [InverseProperty("journal_entrylinked_notarial_acts")]
    public virtual NotarialAct LinkedNotarialAct { get; set; } = null!;

    [ForeignKey("locked_by_user_id")]
    [InverseProperty("journal_entrylocked_by_users")]
    public virtual User? LockedByUser { get; set; }

    [InverseProperty("linked_journal_entry")]
    public virtual ICollection<NotarialAct> NotarialActs { get; set; } = new List<NotarialAct>();

    [ForeignKey("notary_id")]
    [InverseProperty("journal_entries")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("region_id")]
    [InverseProperty("journal_entries")]
    public virtual Region? Region { get; set; }

    [InverseProperty("journal_entry")]
    public virtual ICollection<RevenueShare> RevenueShares { get; set; } = new List<RevenueShare>();

    [InverseProperty("journal_entry")]
    public virtual ICollection<SealUsageLog> SealUsageLogs { get; set; } = new List<SealUsageLog>();

    [ForeignKey("tenant_id")]
    [InverseProperty("journal_entries")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("journal_entryupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }

    [ForeignKey("voided_by_user_id")]
    [InverseProperty("journal_entryvoided_by_users")]
    public virtual User? VoidedByUser { get; set; }
}

// <auto-enum-partial>
public partial class JournalEntry
{
    public JournalEntryStatus entry_status { get; set; }
}
// </auto-enum-partial>
