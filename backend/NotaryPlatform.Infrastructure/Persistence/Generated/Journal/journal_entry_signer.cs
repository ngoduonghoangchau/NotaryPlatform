using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Journal.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Journal;

/// <summary>
/// Signer and identity information for each journal entry.
/// </summary>
[Table("journal_entry_signers", Schema = "journal")]
[Index("full_legal_name", Name = "ix_journal_entry_signers_full_legal_name")]
[Index("is_primary_signer", Name = "ix_journal_entry_signers_is_primary_signer")]
[Index("journal_entry_id", Name = "ix_journal_entry_signers_journal_entry_id")]
[Index("tenant_id", Name = "ix_journal_entry_signers_tenant_id")]
public partial class JournalEntrySigner
{
    [Key]
    public Guid JournalEntrySignerId { get; set; }

    public Guid TenantId { get; set; }

    public Guid JournalEntryId { get; set; }

    public int SignerOrder { get; set; }

    [StringLength(200)]
    public string FullLegalName { get; set; } = null!;

    [StringLength(150)]
    public string? RoleOrCapacity { get; set; }

    [StringLength(100)]
    public string? SignerRole { get; set; }

    [StringLength(200)]
    public string? AddressLine1 { get; set; }

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(2)]
    public string? CountryCode { get; set; }

    [StringLength(100)]
    public string? IdType { get; set; }

    [StringLength(100)]
    public string? IdNumber { get; set; }

    [StringLength(200)]
    public string? IssuingAuthority { get; set; }

    public DateOnly? IdIssueDate { get; set; }

    public DateOnly? IdExpirationDate { get; set; }

    [StringLength(50)]
    public string VerificationResult { get; set; } = null!;

    [Column(TypeName = "citext")]
    public string? SignerEmail { get; set; }

    [StringLength(30)]
    public string? SignerPhone { get; set; }

    public bool IsPrimarySigner { get; set; }

    public bool IsMissingSignature { get; set; }

    public bool IsMissingThumbprint { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("journal_entry_signer")]
    public virtual JournalEntry JournalEntry { get; set; } = null!;

    [InverseProperty("journal_entry_signer")]
    public virtual ICollection<JournalEntryIdDocument> JournalEntryIdDocuments { get; set; } = new List<JournalEntryIdDocument>();

    [InverseProperty("journal_entry_signer")]
    public virtual ICollection<JournalEntrySignature> JournalEntrySignatures { get; set; } = new List<JournalEntrySignature>();

    [InverseProperty("journal_entry_signer")]
    public virtual ICollection<JournalEntryThumbprint> JournalEntryThumbprints { get; set; } = new List<JournalEntryThumbprint>();

    [ForeignKey("tenant_id")]
    [InverseProperty("journal_entry_signers")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class JournalEntrySigner
{
    public JournalVerificationMethod verification_method { get; set; }
}
// </auto-enum-partial>
