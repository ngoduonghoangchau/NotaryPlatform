using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VActLatestCertificate
{
    public Guid? CertificateId { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? ActId { get; set; }

    [StringLength(100)]
    public string? CertificateNumber { get; set; }

    [StringLength(300)]
    public string? CertificateTitle { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public DateTime? FinalizedAt { get; set; }

    public DateTime? LockedAt { get; set; }
}

// <auto-enum-partial>
public partial class VActLatestCertificate
{
    public CertificateStatus certificate_status { get; set; }
}
// </auto-enum-partial>
