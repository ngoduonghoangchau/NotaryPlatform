using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Many-to-many mapping between users and roles.
/// </summary>
[PrimaryKey("user_id", "role_id")]
[Table("user_roles", Schema = "core")]
[Index("assigned_by_user_id", Name = "ix_user_roles_assigned_by_user_id")]
[Index("role_id", Name = "ix_user_roles_role_id")]
public partial class UserRole
{
    [Key]
    public Guid UserId { get; set; }

    [Key]
    public Guid RoleId { get; set; }

    public Guid? AssignedByUserId { get; set; }

    public DateTime AssignedAt { get; set; }

    [ForeignKey("assigned_by_user_id")]
    [InverseProperty("user_roleassigned_by_users")]
    public virtual User? AssignedByUser { get; set; }

    [ForeignKey("role_id")]
    [InverseProperty("user_roles")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("user_id")]
    [InverseProperty("user_roleusers")]
    public virtual User User { get; set; } = null!;
}
