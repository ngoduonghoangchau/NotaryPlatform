using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;

/// <summary>
/// Builds a valid <see cref="Branch"/> aggregate from fake seed data.
/// Callers must generate all of a tenant's branches in a single
/// <see cref="BranchDataGenerator.Generate"/> call so the state/city pairs are
/// distinct tenant-wide (<c>core.branches</c> has a tenant-wide unique code) —
/// see <c>BranchSeeder</c>.
/// </summary>
public static class BranchBuilder
{
    public static Branch Build(Guid tenantId, Guid organizationId, BranchSeedData data)
    {
        var code = SeedSlug.From($"{data.StateCode}-{data.City}");
        var branch = Branch.Create(tenantId, organizationId, code, data.City);
        branch.UpdateProfile(data.City, data.CountryCode, data.StateCode, data.TimeZoneId);
        return branch;
    }
}
