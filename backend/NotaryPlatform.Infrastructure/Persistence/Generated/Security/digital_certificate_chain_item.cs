using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Security;

/// <summary>
/// Certificate chain items for validation and audit.
/// </summary>
[Table("digital_certificate_chain_items", Schema = "security")]
[Index("digital_certificate_id", Name = "ix_chain_items_certificate_id")]
[Index("chain_order", Name = "ix_chain_items_chain_order")]
[Index("tenant_id", Name = "ix_chain_items_tenant_id")]
public partial class DigitalCertificateChainItem
{
    [Key]
    public Guid ChainItemId { get; set; }

    public Guid TenantId { get; set; }

    public Guid DigitalCertificateId { get; set; }

    public int ChainOrder { get; set; }

    [StringLength(200)]
    public string? SubjectName { get; set; }

    [StringLength(200)]
    public string? IssuerName { get; set; }

    [StringLength(150)]
    public string? SerialNumber { get; set; }

    [StringLength(64)]
    public string? ThumbprintSha256 { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsRoot { get; set; }

    [Column(TypeName = "jsonb")]
    public string Metadata { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("digital_certificate_id")]
    [InverseProperty("digital_certificate_chain_items")]
    public virtual DigitalCertificate DigitalCertificate { get; set; } = null!;

    [ForeignKey("tenant_id")]
    [InverseProperty("digital_certificate_chain_items")]
    public virtual Tenant Tenant { get; set; } = null!;
}
