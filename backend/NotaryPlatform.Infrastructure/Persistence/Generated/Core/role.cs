using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Tenant-scoped roles.
/// </summary>
[Table("roles", Schema = "core")]
[Index("is_active", Name = "ix_roles_is_active")]
[Index("tenant_id", Name = "ix_roles_tenant_id")]
[Index("tenant_id", "role_code", Name = "uq_roles_tenant_code", IsUnique = true)]
public partial class Role
{
    [Key]
    public Guid RoleId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(100)]
    public string RoleCode { get; set; } = null!;

    [StringLength(200)]
    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsSystem { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("role")]
    public virtual ICollection<ComplianceRule> ComplianceRules { get; set; } = new List<ComplianceRule>();

    [InverseProperty("role")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    [ForeignKey("tenant_id")]
    [InverseProperty("roles")]
    public virtual Tenant Tenant { get; set; } = null!;

    [InverseProperty("role")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
