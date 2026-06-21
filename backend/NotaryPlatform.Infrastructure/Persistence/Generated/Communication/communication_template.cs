using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Communication;

/// <summary>
/// Reusable templates for email, SMS, call scripts, and meetings.
/// </summary>
[Table("communication_templates", Schema = "communication")]
[Index("is_active", Name = "ix_templates_is_active")]
[Index("tenant_id", Name = "ix_templates_tenant_id")]
[Index("tenant_id", "template_code", Name = "uq_templates_tenant_code", IsUnique = true)]
public partial class CommunicationTemplate
{
    [Key]
    public Guid TemplateId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string TemplateCode { get; set; } = null!;

    [StringLength(200)]
    public string TemplateName { get; set; } = null!;

    [StringLength(300)]
    public string? SubjectTemplate { get; set; }

    public string? BodyTextTemplate { get; set; }

    public string? BodyHtmlTemplate { get; set; }

    public bool IsSystem { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "jsonb")]
    public string VariablesSchema { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("template")]
    public virtual ICollection<CommunicationReminder> CommunicationReminders { get; set; } = new List<CommunicationReminder>();

    [ForeignKey("created_by_user_id")]
    [InverseProperty("communication_templatecreated_by_users")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("communication_templates")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("updated_by_user_id")]
    [InverseProperty("communication_templateupdated_by_users")]
    public virtual User? UpdatedByUser { get; set; }
}

// <auto-enum-partial>
public partial class CommunicationTemplate
{
    public ChannelType channel_type { get; set; }
    public TemplateType template_type { get; set; }
}
// </auto-enum-partial>
