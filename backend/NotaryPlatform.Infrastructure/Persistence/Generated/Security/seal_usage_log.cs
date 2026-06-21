using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// Traceable usage logs for seal and certificate operations.
/// </summary>
[Table("seal_usage_logs", Schema = "security")]
[Index("digital_certificate_id", Name = "ix_usage_logs_certificate_id")]
[Index("journal_entry_id", Name = "ix_usage_logs_journal_entry_id")]
[Index("notarial_act_id", Name = "ix_usage_logs_notarial_act_id")]
[Index("seal_id", Name = "ix_usage_logs_seal_id")]
[Index("tenant_id", Name = "ix_usage_logs_tenant_id")]
[Index("used_at", Name = "ix_usage_logs_used_at")]
[Index("tenant_id", "usage_code", Name = "uq_usage_logs_tenant_code", IsUnique = true)]
public partial class SealUsageLog
{
    [Key]
    public Guid UsageLogId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string UsageCode { get; set; } = null!;

    public Guid? SealId { get; set; }

    public Guid? DigitalCertificateId { get; set; }

    public Guid? PolicyId { get; set; }

    public Guid? IncidentId { get; set; }

    public Guid? NotarialActId { get; set; }

    public Guid? JournalEntryId { get; set; }

    public Guid UsedByUserId { get; set; }

    public Guid? NotaryId { get; set; }

    public DateTime UsedAt { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public IPAddress? SourceIp { get; set; }

    public string? UserAgent { get; set; }

    public Guid? DeviceId { get; set; }

    public string? Reason { get; set; }

    public string? DenialReason { get; set; }

    [Column(TypeName = "jsonb")]
    public string Evidence { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("device_id")]
    [InverseProperty("seal_usage_logs")]
    public virtual TrustedDevice? Device { get; set; }

    [ForeignKey("digital_certificate_id")]
    [InverseProperty("seal_usage_logs")]
    public virtual DigitalCertificate? DigitalCertificate { get; set; }

    [ForeignKey("incident_id")]
    [InverseProperty("seal_usage_logs")]
    public virtual SecurityIncident? Incident { get; set; }

    [ForeignKey("journal_entry_id")]
    [InverseProperty("seal_usage_logs")]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey("notarial_act_id")]
    [InverseProperty("seal_usage_logs")]
    public virtual NotarialAct? NotarialAct { get; set; }

    [ForeignKey("notary_id")]
    [InverseProperty("seal_usage_logs")]
    public virtual Notary? Notary { get; set; }

    [ForeignKey("policy_id")]
    [InverseProperty("seal_usage_logs")]
    public virtual SealAccessPolicy? Policy { get; set; }

    [ForeignKey("seal_id")]
    [InverseProperty("seal_usage_logs")]
    public virtual Seal? Seal { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("seal_usage_logs")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("used_by_user_id")]
    [InverseProperty("seal_usage_logs")]
    public virtual User UsedByUser { get; set; } = null!;
}

// <auto-enum-partial>
public partial class SealUsageLog
{
    public UsageActionType action_type { get; set; }
    public UsageResultType result { get; set; }
}
// </auto-enum-partial>
