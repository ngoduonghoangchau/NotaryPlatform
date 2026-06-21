using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VCertificateInventory
{
    public Guid? DigitalCertificateId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(50)]
    public string? CertificateCode { get; set; }

    [StringLength(200)]
    public string? CertificateName { get; set; }

    public Guid? NotaryId { get; set; }

    [StringLength(200)]
    public string? NotaryName { get; set; }

    [StringLength(200)]
    public string? ProviderName { get; set; }

    [StringLength(150)]
    public string? SerialNumber { get; set; }

    [StringLength(100)]
    public string? CryptographicAlgorithm { get; set; }

    [StringLength(100)]
    public string? KeyStorageMethod { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime? ReplacedAt { get; set; }
}

// <auto-enum-partial>
public partial class VCertificateInventory
{
    public CertificateStatus status { get; set; }
}
// </auto-enum-partial>
