using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;
using EfUserRole = NotaryPlatform.Infrastructure.Persistence.Generated.Core.UserRole;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Core;

/// <summary>
/// Seeds <c>core.user_roles</c>, assigning each tenant's staff exactly one
/// system role, distributed round-robin across the roles created by
/// <see cref="RoleSeeder"/>.
/// </summary>
public sealed class UserRoleSeeder : ISeeder
{
    public string Name => "core.user_roles";

    public int Order => 1000;

    public IReadOnlyCollection<SeedProfile> SupportedProfiles { get; } =
        [SeedProfile.Development, SeedProfile.Demo, SeedProfile.Staging];

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        foreach (var tenantId in context.Registry.TenantIds)
        {
            var roleIds = context.Registry.RolesForTenant(tenantId).Values.ToList();
            var userIds = context.Registry.UserIdsForTenant(tenantId).ToList();

            if (roleIds.Count == 0 || userIds.Count == 0)
            {
                continue;
            }

            var existingPairs = await context.DbContext.UserRoles
                .AsNoTracking()
                .Where(ur => userIds.Contains(ur.UserId))
                .Select(ur => new { ur.UserId, ur.RoleId })
                .ToListAsync(cancellationToken);

            var existingSet = existingPairs.Select(p => (p.UserId, p.RoleId)).ToHashSet();

            for (var i = 0; i < userIds.Count; i++)
            {
                var userId = userIds[i];
                var roleId = roleIds[i % roleIds.Count];

                if (existingSet.Contains((userId, roleId)))
                {
                    continue;
                }

                var freshUserAvailable = context.Registry.TryGetFreshUser(userId, out var freshUser);
                var freshRoleAvailable = context.Registry.TryGetFreshRole(roleId, out var freshRole);

                if (freshUserAvailable && freshRoleAvailable)
                {
                    freshUser.AssignRole(freshRole);
                }

                context.DbContext.UserRoles.Add(new EfUserRole
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }
        }

        await context.DbContext.SaveChangesAsync(cancellationToken);
    }
}
