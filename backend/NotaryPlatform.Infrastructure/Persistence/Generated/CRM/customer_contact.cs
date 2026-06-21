using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Multiple contacts per customer/account.
/// </summary>
[Table("customer_contacts", Schema = "crm")]
[Index("customer_id", Name = "ix_customer_contacts_customer_id")]
[Index("is_primary", Name = "ix_customer_contacts_is_primary")]
[Index("tenant_id", Name = "ix_customer_contacts_tenant_id")]
[Index("customer_id", "contact_code", Name = "uq_customer_contacts_customer_code", IsUnique = true)]
[Index("customer_id", "email", Name = "uq_customer_contacts_customer_email", IsUnique = true)]
public partial class CustomerContact
{
    [Key]
    public Guid ContactId { get; set; }

    public Guid TenantId { get; set; }

    public Guid CustomerId { get; set; }

    [StringLength(50)]
    public string ContactCode { get; set; } = null!;

    [StringLength(200)]
    public string FullName { get; set; } = null!;

    [StringLength(150)]
    public string? JobTitle { get; set; }

    [Column(TypeName = "citext")]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(30)]
    public string? MobilePhone { get; set; }

    [StringLength(120)]
    public string? Department { get; set; }

    public bool IsPrimary { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "jsonb")]
    public string Settings { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("customer_contact")]
    public virtual ICollection<CommunicationParticipant> CommunicationParticipants { get; set; } = new List<CommunicationParticipant>();

    [InverseProperty("customer_contact")]
    public virtual ICollection<CommunicationThread> CommunicationThreads { get; set; } = new List<CommunicationThread>();

    [ForeignKey("customer_id")]
    [InverseProperty("customer_contact")]
    public virtual Customer Customer { get; set; } = null!;

    [InverseProperty("contact")]
    public virtual CustomerContactPreference? CustomerContactPreference { get; set; }

    [InverseProperty("customer_contact")]
    public virtual ICollection<InternalNote> InternalNotes { get; set; } = new List<InternalNote>();

    [InverseProperty("customer_contact")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("recipient_contact")]
    public virtual ICollection<JobReminder> JobReminders { get; set; } = new List<JobReminder>();

    [InverseProperty("customer_contact")]
    public virtual ICollection<JobRequest> JobRequests { get; set; } = new List<JobRequest>();

    [InverseProperty("customer_contact")]
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    [InverseProperty("customer_contact")]
    public virtual ICollection<NotarialAct> NotarialActs { get; set; } = new List<NotarialAct>();

    [InverseProperty("customer_contact")]
    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();

    [ForeignKey("tenant_id")]
    [InverseProperty("customer_contacts")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class CustomerContact
{
    public ContactRole role { get; set; }
    public ContactStatus status { get; set; }
}
// </auto-enum-partial>
