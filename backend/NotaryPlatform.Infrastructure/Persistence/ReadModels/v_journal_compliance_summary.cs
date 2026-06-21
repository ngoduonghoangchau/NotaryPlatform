using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VJournalComplianceSummary
{
    public Guid? TenantId { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public long? TotalEntries { get; set; }

    public long? LockedEntries { get; set; }

    public long? MissingSignatureEntries { get; set; }

    public long? MissingThumbprintEntries { get; set; }

    public long? VoidedEntries { get; set; }
}
