using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VCustomerPrimaryContact
{
    public Guid? CustomerId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(200)]
    public string? CustomerName { get; set; }

    public Guid? ContactId { get; set; }

    [StringLength(200)]
    public string? FullName { get; set; }

    [Column(TypeName = "citext")]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(30)]
    public string? MobilePhone { get; set; }
}

// <auto-enum-partial>
public partial class VCustomerPrimaryContact
{
    public ContactRole role { get; set; }
}
// </auto-enum-partial>
