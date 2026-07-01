using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Application.Common.Interfaces;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;

namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// Queries PostgreSQL to resolve a user's roles and expanded permission codes.
/// Implements IPermissionService (defined in Application).
/// Called during login / token-refresh — not on every request.
///
/// LEARNING — Permission embedding strategy (chosen for this project):
///   Embed all permission codes in the JWT at login time.
///   ✅ Zero DB lookups on every request — fully stateless.
///   ❌ Permission changes take effect only after the next login or token refresh.
///   ❌ JWT payload grows with many permissions (mitigated by short permission codes).
///   This trade-off is appropriate when admin changes are infrequent and the
///   60-minute access token expiry bounds the staleness window.
///
/// LEARNING — RBAC query path:
///   users → user_roles → roles (Role.TenantId filter) → role_permissions → permissions
///   Permissions are global (no TenantId on Permission).
///   Roles are tenant-scoped (Role.TenantId), which naturally constrains what
///   permissions a user can hold within a specific tenant context.
/// </summary>
public sealed class PermissionService : IPermissionService
{
    private readonly NotaryPlatformDbContext _context;

    public PermissionService(NotaryPlatformDbContext context) => _context = context;

    public async Task<IReadOnlyList<string>> GetPermissionsAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur =>
                ur.UserId == userId
                && ur.Role.TenantId == tenantId
                && ur.Role.IsActive
                && ur.Role.DeletedAt == null)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Where(rp =>
                rp.Permission.IsActive
                && rp.Permission.DeletedAt == null)
            .Select(rp => rp.Permission.PermissionCode)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur =>
                ur.UserId == userId
                && ur.Role.TenantId == tenantId
                && ur.Role.IsActive
                && ur.Role.DeletedAt == null)
            .Select(ur => ur.Role.RoleCode)
            .ToListAsync(cancellationToken);
    }
}
