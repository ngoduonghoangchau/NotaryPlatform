using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;

/// <summary>
/// Generated notarial certificate content and lock state.
/// </summary>
[Table("notarial_certificates", Schema = "notarial")]
[Index("act_id", Name = "ix_notarial_certificates_act_id")]
[Index("finalized_at", Name = "ix_notarial_certificates_finalized_at")]
[Index("state_code", Name = "ix_notarial_certificates_state_code")]
[Index("tenant_id", Name = "ix_notarial_certificates_tenant_id")]
[Index("act_id", "certificate_number", Name = "uq_notarial_certificates_act_number", IsUnique = true)]
public partial class NotarialCertificate
{
    [Key]
    public Guid CertificateId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ActId { get; set; }

    [StringLength(100)]
    public string CertificateNumber { get; set; } = null!;

    [StringLength(100)]
    public string? CertificateTemplateCode { get; set; }

    [StringLength(300)]
    public string? CertificateTitle { get; set; }

    public string CertificateBody { get; set; } = null!;

    [StringLength(300)]
    public string? VenueText { get; set; }

    [StringLength(2)]
    public string StateCode { get; set; } = null!;

    [StringLength(100)]
    public string? CountyName { get; set; }

    [StringLength(100)]
    public string? CityName { get; set; }

    public DateOnly? IssueDate { get; set; }

    public TimeOnly? IssueTime { get; set; }

    public string? SealReference { get; set; }

    public string? DigitalCertificateReference { get; set; }

    public string? CryptographicSignature { get; set; }

    public DateTime? GeneratedAt { get; set; }

    public DateTime? PreviewedAt { get; set; }

    public DateTime? FinalizedAt { get; set; }

    public DateTime? LockedAt { get; set; }

    public Guid? GeneratedByUserId { get; set; }

    public Guid? FinalizedByUserId { get; set; }

    public Guid? LockedByUserId { get; set; }

    public Guid? LinkedJournalEntryId { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("act_id")]
    [InverseProperty("notarial_certificates")]
    public virtual NotarialAct Act { get; set; } = null!;

    [ForeignKey("finalized_by_user_id")]
    [InverseProperty("notarial_certificatefinalized_by_users")]
    public virtual User? FinalizedByUser { get; set; }

    [ForeignKey("generated_by_user_id")]
    [InverseProperty("notarial_certificategenerated_by_users")]
    public virtual User? GeneratedByUser { get; set; }

    [InverseProperty("linked_certificate")]
    public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();

    [ForeignKey("locked_by_user_id")]
    [InverseProperty("notarial_certificatelocked_by_users")]
    public virtual User? LockedByUser { get; set; }

    [InverseProperty("linked_certificate")]
    public virtual ICollection<NotarialAct> NotarialActs { get; set; } = new List<NotarialAct>();

    [ForeignKey("tenant_id")]
    [InverseProperty("notarial_certificates")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class NotarialCertificate
{
    public CertificateStatus certificate_status { get; set; }
}
// </auto-enum-partial>
