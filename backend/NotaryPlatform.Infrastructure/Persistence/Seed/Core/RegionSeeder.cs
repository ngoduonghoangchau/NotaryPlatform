using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;
using EfRegion = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Region;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Core;

/// <summary>
/// Seeds <c>core.regions</c> — dispatch/compliance coverage areas. All of a
/// tenant's regions are generated in one call (names distinct tenant-wide,
/// bounded by the curated region pool) and distributed round-robin across
/// its organizations.
/// </summary>
public sealed class RegionSeeder : ISeeder
{
    private readonly RegionDataGenerator _generator;

    public RegionSeeder(RegionDataGenerator generator) => _generator = generator;

    public string Name => "core.regions";

    public int Order => 500;

    public IReadOnlyCollection<SeedProfile> SupportedProfiles { get; } =
        [SeedProfile.Development, SeedProfile.Demo, SeedProfile.Staging];

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        var perOrganization = context.VolumePlan.RegionsPerOrganization;
        if (perOrganization == 0)
        {
            return;
        }

        foreach (var tenantId in context.Registry.TenantIds)
        {
            var organizationIds = await context.DbContext.Organizations
                .AsNoTracking()
                .Where(o => o.TenantId == tenantId)
                .Select(o => o.OrganizationId)
                .ToListAsync(cancellationToken);

            if (organizationIds.Count == 0)
            {
                continue;
            }

            var data = _generator.Generate(organizationIds.Count * perOrganization);
            var candidateCodes = data.Select(d => SeedSlug.From(d.Name)).ToList();

            var existingByCode = await context.DbContext.Regions
                .AsNoTracking()
                .Where(r => r.TenantId == tenantId && candidateCodes.Contains(r.RegionCode))
                .ToDictionaryAsync(r => r.RegionCode, r => r.RegionId, StringComparer.OrdinalIgnoreCase, cancellationToken);

            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                var code = SeedSlug.From(item.Name);
                var organizationId = organizationIds[i % organizationIds.Count];

                if (existingByCode.TryGetValue(code, out var existingId))
                {
                    context.Registry.RegisterRegion(tenantId, code, existingId);
                    continue;
                }

                var region = RegionBuilder.Build(tenantId, organizationId, item);

                context.DbContext.Regions.Add(new EfRegion
                {
                    RegionId = region.Id,
                    TenantId = tenantId,
                    OrganizationId = organizationId,
                    RegionCode = region.Code,
                    RegionName = region.Name,
                    CountryCode = region.CountryCode,
                    StateCode = region.StateCode,
                    Description = item.Description,
                    status = region.Status
                });

                context.Registry.RegisterRegion(tenantId, code, region.Id);
            }
        }

        await context.DbContext.SaveChangesAsync(cancellationToken);
    }
}
