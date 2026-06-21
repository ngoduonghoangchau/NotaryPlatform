using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Identity;

/// <summary>
/// Human-readable notes for senior admins and compliance.
/// </summary>
[Table("notary_audit_notes", Schema = "identity")]
[Index("created_at", Name = "ix_notary_audit_notes_created_at")]
[Index("notary_id", Name = "ix_notary_audit_notes_notary_id")]
[Index("tenant_id", Name = "ix_notary_audit_notes_tenant_id")]
public partial class NotaryAuditNote
{
    [Key]
    public Guid AuditNoteId { get; set; }

    public Guid TenantId { get; set; }

    public Guid NotaryId { get; set; }

    [StringLength(200)]
    public string? NoteTitle { get; set; }

    public string NoteBody { get; set; } = null!;

    [StringLength(50)]
    public string VisibilityLevel { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("notary_audit_notecreated_by_users")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("notary_id")]
    [InverseProperty("notary_audit_notes")]
    public virtual Notary Notary { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("notary_audit_notes")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("notary_audit_noteupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}
