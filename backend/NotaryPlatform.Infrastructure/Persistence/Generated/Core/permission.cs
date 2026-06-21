using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Global permission catalog.
/// </summary>
[Table("permissions", Schema = "core")]
[Index("is_active", Name = "ix_permissions_is_active")]
[Index("module_name", Name = "ix_permissions_module_name")]
[Index("permission_code", Name = "uq_permissions_code", IsUnique = true)]
public partial class Permission
{
    [Key]
    public Guid PermissionId { get; set; }

    [StringLength(150)]
    public string PermissionCode { get; set; } = null!;

    [StringLength(100)]
    public string ModuleName { get; set; } = null!;

    [StringLength(200)]
    public string PermissionName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("permission")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
