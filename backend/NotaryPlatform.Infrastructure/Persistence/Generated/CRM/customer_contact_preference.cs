using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Notification and communication preferences for contacts.
/// </summary>
[Table("customer_contact_preferences", Schema = "crm")]
[Index("tenant_id", Name = "ix_customer_contact_preferences_tenant_id")]
public partial class CustomerContactPreference
{
    [Key]
    public Guid ContactId { get; set; }

    public Guid TenantId { get; set; }

    public bool EmailEnabled { get; set; }

    public bool SmsEnabled { get; set; }

    public bool InAppEnabled { get; set; }

    public bool MarketingEnabled { get; set; }

    public bool OperationalEnabled { get; set; }

    public bool BillingEnabled { get; set; }

    [StringLength(20)]
    public string PreferredLanguage { get; set; } = null!;

    public TimeOnly? QuietHoursStart { get; set; }

    public TimeOnly? QuietHoursEnd { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("contact_id")]
    [InverseProperty("customer_contact_preference")]
    public virtual CustomerContact Contact { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("customer_contact_preferences")]
    public virtual Tenant Tenant { get; set; } = null!;
}
