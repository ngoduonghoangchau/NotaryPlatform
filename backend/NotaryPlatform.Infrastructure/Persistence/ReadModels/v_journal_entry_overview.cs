using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Journal.Enums;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VJournalEntryOverview
{
    public Guid? JournalEntryId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(50)]
    public string? EntryCode { get; set; }

    public Guid? ActId { get; set; }

    [StringLength(50)]
    public string? ActCode { get; set; }

    public Guid? NotaryId { get; set; }

    [StringLength(200)]
    public string? NotaryName { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public DateOnly? EntryDate { get; set; }

    public DateTime? EntryTimestamp { get; set; }

    public int? SignerCount { get; set; }

    [Precision(18, 2)]
    public decimal? FeeCharged { get; set; }

    [StringLength(3)]
    public string? CurrencyCode { get; set; }

    public bool? IsComplete { get; set; }

    public bool? IsLocked { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? LockedAt { get; set; }

    public DateTime? VoidedAt { get; set; }
}

// <auto-enum-partial>
public partial class VJournalEntryOverview
{
    public NotarialActType act_type { get; set; }
    public JournalEntryStatus entry_status { get; set; }
}
// </auto-enum-partial>
