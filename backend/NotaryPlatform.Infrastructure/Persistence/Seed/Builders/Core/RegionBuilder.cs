using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;

/// <summary>
/// Builds a valid <see cref="Region"/> aggregate from fake seed data.
/// Callers must generate all of a tenant's regions in a single
/// <see cref="RegionDataGenerator.Generate"/> call so the names are distinct
/// tenant-wide (<c>core.regions</c> has a tenant-wide unique code) — see
/// <c>RegionSeeder</c>.
/// </summary>
public static class RegionBuilder
{
    public static Region Build(Guid tenantId, Guid organizationId, RegionSeedData data)
    {
        var code = SeedSlug.From(data.Name);
        var region = Region.Create(tenantId, organizationId, code, data.Name);
        region.UpdateProfile(data.Name, data.CountryCode, data.StateCode);
        return region;
    }
}
