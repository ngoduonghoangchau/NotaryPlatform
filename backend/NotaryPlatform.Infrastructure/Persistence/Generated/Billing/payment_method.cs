using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Billing;

/// <summary>
/// Stored customer payment methods.
/// </summary>
[Table("payment_methods", Schema = "billing")]
[Index("customer_id", Name = "ix_payment_methods_customer_id")]
[Index("tenant_id", Name = "ix_payment_methods_tenant_id")]
[Index("customer_id", "method_code", Name = "uq_payment_methods_customer_code", IsUnique = true)]
public partial class PaymentMethod
{
    [Key]
    public Guid PaymentMethodId { get; set; }

    public Guid TenantId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? CustomerContactId { get; set; }

    [StringLength(50)]
    public string MethodCode { get; set; } = null!;

    [StringLength(200)]
    public string? ProviderName { get; set; }

    public string? ProviderCustomerRef { get; set; }

    public string? ProviderMethodRef { get; set; }

    [StringLength(200)]
    public string? DisplayName { get; set; }

    [StringLength(4)]
    public string? Last4 { get; set; }

    [StringLength(50)]
    public string? CardBrand { get; set; }

    public int? ExpMonth { get; set; }

    public int? ExpYear { get; set; }

    [StringLength(20)]
    public string? BankAccountMask { get; set; }

    [StringLength(20)]
    public string? BankRoutingMask { get; set; }

    public bool IsDefault { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public DateTime? TokenizedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("payment_method")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("customer_contact_id")]
    [InverseProperty("payment_methods")]
    public virtual CustomerContact? CustomerContact { get; set; }

    [InverseProperty("payment_method")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("tenant_id")]
    [InverseProperty("payment_methods")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class PaymentMethod
{
    public PaymentMethodType method_type { get; set; }
    public PaymentStatus status { get; set; }
}
// </auto-enum-partial>
