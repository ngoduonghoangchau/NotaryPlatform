using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VActiveUser
{
    public Guid? UserId { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? OrganizationId { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? TeamId { get; set; }

    [StringLength(50)]
    public string? UserCode { get; set; }

    [Column(TypeName = "citext")]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(200)]
    public string? DisplayName { get; set; }

    [StringLength(20)]
    public string? Locale { get; set; }

    [StringLength(64)]
    public string? TimeZone { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

// <auto-enum-partial>
public partial class VActiveUser
{
    public UserStatus status { get; set; }
}
// </auto-enum-partial>
