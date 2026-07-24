using System.Globalization;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Application.Shared.Constants;
using NotaryPlatform.Infrastructure.Authorization.PermissionMaps;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;
using EfPermission = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Permission;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Core;

/// <summary>
/// Seeds <c>core.permissions</c> from every <c>const string</c> declared under
/// <see cref="PermissionCodes"/>, mirroring the reflection pattern already used
/// by <c>PolicyRegistrationExtensions</c> to register ASP.NET Core policies.
/// Tenant-independent — the only seeder that participates in every profile,
/// including <see cref="SeedProfile.ProductionBootstrap"/>.
/// </summary>
public sealed class PermissionSeeder : ISeeder
{
    public string Name => "core.permissions";

    public int Order => 100;

    public IReadOnlyCollection<SeedProfile> SupportedProfiles { get; } =
    [
        SeedProfile.Development, SeedProfile.Demo, SeedProfile.Staging, SeedProfile.ProductionBootstrap
    ];

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        var existingCodes = await context.DbContext.Permissions
            .AsNoTracking()
            .Select(p => p.PermissionCode)
            .ToListAsync(cancellationToken);

        var existingCodeSet = new HashSet<string>(existingCodes, StringComparer.OrdinalIgnoreCase);

        foreach (var code in CollectAllPermissionCodes())
        {
            if (existingCodeSet.Contains(code))
            {
                var existingId = await context.DbContext.Permissions
                    .AsNoTracking()
                    .Where(p => p.PermissionCode == code)
                    .Select(p => p.PermissionId)
                    .SingleAsync(cancellationToken);

                // Already persisted from a previous run — record the id only.
                // Do NOT rebuild via PermissionBuilder here: that would mint a
                // new random Guid that would never match the row above.
                context.Registry.RegisterPermission(code, existingId, freshPermission: null);
                continue;
            }

            var permission = PermissionBuilder.Build(code, HumanizeName(code), Describe(code));

            context.DbContext.Permissions.Add(new EfPermission
            {
                PermissionId = permission.Id,
                PermissionCode = permission.Code,
                ModuleName = ModuleOf(code),
                PermissionName = permission.Name,
                Description = permission.Description,
                IsActive = permission.IsActive
            });

            context.Registry.RegisterPermission(code, permission.Id, permission);
        }

        await context.DbContext.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<string> CollectAllPermissionCodes() =>
        typeof(PermissionCodes)
            .GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static))
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .Distinct(StringComparer.OrdinalIgnoreCase);

    private static string ModuleOf(string code) => code.Split('.', 2)[0];

    private static string HumanizeName(string code)
    {
        var segments = code.Split('.');
        var words = segments[1..].SelectMany(s => s.Split('-'));
        var textInfo = CultureInfo.InvariantCulture.TextInfo;
        return string.Join(' ', words.Select(w => textInfo.ToTitleCase(w)));
    }

    private static string Describe(string code)
    {
        var segments = code.Split('.');
        var module = segments[0];
        var action = segments[^1].Replace('-', ' ');
        var resource = string.Join(' ', segments[1..^1]).Replace('-', ' ');
        return $"Grants ability to {action} {resource} within the {module} module.";
    }
}
