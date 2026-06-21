using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Journal.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VJournalEntrySignerOverview
{
    public Guid? JournalEntrySignerId { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? JournalEntryId { get; set; }

    public int? SignerOrder { get; set; }

    [StringLength(200)]
    public string? FullLegalName { get; set; }

    [StringLength(150)]
    public string? RoleOrCapacity { get; set; }

    [StringLength(100)]
    public string? SignerRole { get; set; }

    [StringLength(50)]
    public string? VerificationResult { get; set; }

    public bool? IsPrimarySigner { get; set; }

    public bool? IsMissingSignature { get; set; }

    public bool? IsMissingThumbprint { get; set; }
}

// <auto-enum-partial>
public partial class VJournalEntrySignerOverview
{
    public JournalVerificationMethod verification_method { get; set; }
}
// </auto-enum-partial>
