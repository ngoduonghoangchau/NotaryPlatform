using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Many-to-many mapping between roles and permissions.
/// </summary>
[PrimaryKey("role_id", "permission_id")]
[Table("role_permissions", Schema = "core")]
[Index("permission_id", Name = "ix_role_permissions_permission_id")]
public partial class RolePermission
{
    [Key]
    public Guid RoleId { get; set; }

    [Key]
    public Guid PermissionId { get; set; }

    public DateTime GrantedAt { get; set; }

    [ForeignKey("permission_id")]
    [InverseProperty("role_permissions")]
    public virtual Permission Permission { get; set; } = null!;

    [ForeignKey("role_id")]
    [InverseProperty("role_permissions")]
    public virtual Role Role { get; set; } = null!;
}
