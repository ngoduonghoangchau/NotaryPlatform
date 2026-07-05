using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Authorization.PermissionMaps;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;
using EfRole = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Role;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Core;

/// <summary>
/// Seeds <c>core.roles</c> with the built-in, tenant-scoped role catalog
/// defined by <see cref="RolePermissionMap.RoleCodes"/> — one copy per tenant.
/// </summary>
public sealed class RoleSeeder : ISeeder
{
    private static readonly (string Code, string Name, string Description)[] SystemRoles =
    [
        (RolePermissionMap.RoleCodes.TenantAdmin, "Tenant Administrator", "Full administrative access within a single tenant."),
        (RolePermissionMap.RoleCodes.ComplianceOfficer, "Compliance Officer", "Read-everything plus manage compliance, incidents, and audit output."),
        (RolePermissionMap.RoleCodes.Dispatcher, "Dispatcher", "Assigns Notaries to jobs and manages the scheduling board."),
        (RolePermissionMap.RoleCodes.Notary, "Notary", "Executes notarial acts and manages own journal entries."),
        (RolePermissionMap.RoleCodes.AccountManager, "Account Manager", "Owns B2B customer accounts, creates jobs, reads billing."),
        (RolePermissionMap.RoleCodes.Finance, "Finance", "Manages invoices, payments, and revenue sharing."),
        (RolePermissionMap.RoleCodes.OperationsManager, "Operations Manager", "Oversees all scheduling, dispatch, and workforce operations.")
    ];

    public string Name => "core.roles";

    public int Order => 700;

    public IReadOnlyCollection<SeedProfile> SupportedProfiles { get; } =
        [SeedProfile.Development, SeedProfile.Demo, SeedProfile.Staging];

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        foreach (var tenantId in context.Registry.TenantIds)
        {
            var codes = SystemRoles.Select(r => r.Code).ToList();

            var existingByCode = await context.DbContext.Roles
                .AsNoTracking()
                .Where(r => r.TenantId == tenantId && codes.Contains(r.RoleCode))
                .ToDictionaryAsync(r => r.RoleCode, r => r.RoleId, StringComparer.OrdinalIgnoreCase, cancellationToken);

            foreach (var (code, roleName, description) in SystemRoles)
            {
                if (existingByCode.TryGetValue(code, out var existingId))
                {
                    context.Registry.RegisterRole(tenantId, code, existingId, freshRole: null);
                    continue;
                }

                var role = RoleBuilder.Build(tenantId, code, roleName, description);

                context.DbContext.Roles.Add(new EfRole
                {
                    RoleId = role.Id,
                    TenantId = tenantId,
                    RoleCode = role.Code,
                    RoleName = role.Name,
                    Description = role.Description,
                    IsSystem = role.IsSystem,
                    IsActive = role.IsActive
                });

                context.Registry.RegisterRole(tenantId, code, role.Id, role);
            }
        }

        await context.DbContext.SaveChangesAsync(cancellationToken);
    }
}
