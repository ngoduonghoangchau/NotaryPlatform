using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Authorization.PermissionMaps;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;
using EfRolePermission = NotaryPlatform.Infrastructure.Persistence.Generated.Core.RolePermission;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Core;

/// <summary>
/// Seeds <c>core.role_permissions</c> from <see cref="RolePermissionMap.DefaultPermissions"/>,
/// wiring each tenant's system roles to the global permission catalog seeded by
/// <see cref="PermissionSeeder"/>.
/// </summary>
public sealed class RolePermissionSeeder : ISeeder
{
    public string Name => "core.role_permissions";

    public int Order => 800;

    public IReadOnlyCollection<SeedProfile> SupportedProfiles { get; } =
        [SeedProfile.Development, SeedProfile.Demo, SeedProfile.Staging];

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        foreach (var tenantId in context.Registry.TenantIds)
        {
            foreach (var (roleCode, permissionCodes) in RolePermissionMap.DefaultPermissions)
            {
                if (!context.Registry.TryGetRoleId(tenantId, roleCode, out var roleId))
                {
                    continue;
                }

                var permissionIds = permissionCodes
                    .Select(code => context.Registry.TryGetPermissionId(code, out var id) ? id : (Guid?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .ToList();

                var existingPairs = await context.DbContext.RolePermissions
                    .AsNoTracking()
                    .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
                    .Select(rp => rp.PermissionId)
                    .ToListAsync(cancellationToken);

                var existingSet = new HashSet<Guid>(existingPairs);

                var freshRoleAvailable = context.Registry.TryGetFreshRole(roleId, out var freshRole);

                foreach (var permissionId in permissionIds)
                {
                    if (existingSet.Contains(permissionId))
                    {
                        continue;
                    }

                    // Enforce the aggregate's own dedup invariant when both sides were
                    // created this run; otherwise fall back to the idempotency check above.
                    if (freshRoleAvailable && context.Registry.TryGetFreshPermission(permissionId, out var freshPermission))
                    {
                        freshRole.AssignPermission(freshPermission);
                    }

                    context.DbContext.RolePermissions.Add(new EfRolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId
                    });
                }
            }
        }

        await context.DbContext.SaveChangesAsync(cancellationToken);
    }
}
