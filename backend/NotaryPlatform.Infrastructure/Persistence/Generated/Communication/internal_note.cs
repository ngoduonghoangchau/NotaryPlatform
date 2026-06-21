using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Communication;

/// <summary>
/// Internal notes for sales, operations, and compliance.
/// </summary>
[Table("internal_notes", Schema = "communication")]
[Index("created_at", Name = "ix_internal_notes_created_at")]
[Index("customer_id", Name = "ix_internal_notes_customer_id")]
[Index("job_id", Name = "ix_internal_notes_job_id")]
[Index("tenant_id", Name = "ix_internal_notes_tenant_id")]
[Index("thread_id", Name = "ix_internal_notes_thread_id")]
[Index("tenant_id", "note_code", Name = "uq_internal_notes_tenant_code", IsUnique = true)]
public partial class InternalNote
{
    [Key]
    public Guid InternalNoteId { get; set; }

    public Guid TenantId { get; set; }

    public Guid? ThreadId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? CustomerContactId { get; set; }

    public Guid? JobId { get; set; }

    public Guid? NotarialActId { get; set; }

    public Guid? InvoiceId { get; set; }

    public Guid? IncidentId { get; set; }

    [StringLength(50)]
    public string NoteCode { get; set; } = null!;

    [StringLength(200)]
    public string? Title { get; set; }

    public string Body { get; set; } = null!;

    public bool IsPinned { get; set; }

    public bool IsComplianceNote { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("created_by_user_id")]
    [InverseProperty("internal_notecreated_by_users")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("customer_id")]
    [InverseProperty("internal_notes")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("customer_contact_id")]
    [InverseProperty("internal_notes")]
    public virtual CustomerContact? CustomerContact { get; set; }

    [ForeignKey("incident_id")]
    [InverseProperty("internal_notes")]
    public virtual SecurityIncident? Incident { get; set; }

    [ForeignKey("invoice_id")]
    [InverseProperty("internal_notes")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("job_id")]
    [InverseProperty("internal_notes")]
    public virtual Job? Job { get; set; }

    [ForeignKey("notarial_act_id")]
    [InverseProperty("internal_notes")]
    public virtual NotarialAct? NotarialAct { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("internal_notes")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("thread_id")]
    [InverseProperty("internal_notes")]
    public virtual CommunicationThread? Thread { get; set; }

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("internal_noteupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class InternalNote
{
    public NoteVisibility visibility { get; set; }
}
// </auto-enum-partial>
