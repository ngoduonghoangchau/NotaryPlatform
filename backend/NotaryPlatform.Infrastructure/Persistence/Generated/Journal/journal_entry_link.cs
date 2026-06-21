using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Journal;

/// <summary>
/// Optional generic links to related entities.
/// </summary>
[Table("journal_entry_links", Schema = "journal")]
[Index("journal_entry_id", Name = "ix_journal_entry_links_entry_id")]
[Index("linked_entity_type", "linked_entity_id", Name = "ix_journal_entry_links_linked_entity")]
[Index("tenant_id", Name = "ix_journal_entry_links_tenant_id")]
public partial class JournalEntryLink
{
    [Key]
    public Guid JournalEntryLinkId { get; set; }

    public Guid TenantId { get; set; }

    public Guid JournalEntryId { get; set; }

    [StringLength(100)]
    public string LinkType { get; set; } = null!;

    [StringLength(100)]
    public string LinkedEntityType { get; set; } = null!;

    public Guid LinkedEntityId { get; set; }

    [StringLength(100)]
    public string? LinkedEntityCode { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("journal_entry_links")]
    public virtual JournalEntry JournalEntry { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("journal_entry_links")]
    public virtual Tenant Tenant { get; set; } = null!;
}
