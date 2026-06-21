using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Journal.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Journal;

/// <summary>
/// Transfer logs for journal custody and regulator handoff.
/// </summary>
[Table("journal_transfer_logs", Schema = "journal")]
[Index("effective_at", Name = "ix_journal_transfer_logs_effective_at")]
[Index("journal_entry_id", Name = "ix_journal_transfer_logs_entry_id")]
[Index("tenant_id", Name = "ix_journal_transfer_logs_tenant_id")]
[Index("tenant_id", "transfer_code", Name = "uq_journal_transfer_logs_tenant_code", IsUnique = true)]
public partial class JournalTransferLog
{
    [Key]
    public Guid JournalTransferLogId { get; set; }

    public Guid TenantId { get; set; }

    public Guid? JournalEntryId { get; set; }

    [StringLength(50)]
    public string TransferCode { get; set; } = null!;

    [StringLength(50)]
    public string TransferStatus { get; set; } = null!;

    [StringLength(200)]
    public string? TransferredToName { get; set; }

    [StringLength(200)]
    public string? TransferredToEntity { get; set; }

    [StringLength(200)]
    public string? TransferredToContact { get; set; }

    public string? TransferReference { get; set; }

    public DateTime EffectiveAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? Reason { get; set; }

    public Guid RequestedByUserId { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("approved_by_user_id")]
    [InverseProperty("journal_transfer_logapproved_by_users")]
    public virtual User? ApprovedByUser { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("journal_transfer_logs")]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey("requested_by_user_id")]
    [InverseProperty("journal_transfer_logrequested_by_users")]
    public virtual User RequestedByUser { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("journal_transfer_logs")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class JournalTransferLog
{
    public JournalTransferType transfer_type { get; set; }
}
// </auto-enum-partial>
