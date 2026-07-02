using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;
using EfBranch = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Branch;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Core;

/// <summary>
/// Seeds <c>core.branches</c> — offices in real US metro areas. All of a
/// tenant's branches are generated in one call (state/city pairs distinct
/// tenant-wide, bounded by the curated metro pool) and distributed round-robin
/// across its organizations.
/// </summary>
public sealed class BranchSeeder : ISeeder
{
    private readonly BranchDataGenerator _generator;

    public BranchSeeder(BranchDataGenerator generator) => _generator = generator;

    public string Name => "core.branches";

    public int Order => 400;

    public IReadOnlyCollection<SeedProfile> SupportedProfiles { get; } =
        [SeedProfile.Development, SeedProfile.Demo, SeedProfile.Staging];

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        var perOrganization = context.VolumePlan.BranchesPerOrganization;
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
            var candidateCodes = data.Select(d => SeedSlug.From($"{d.StateCode}-{d.City}")).ToList();

            var existingByCode = await context.DbContext.Branches
                .AsNoTracking()
                .Where(b => b.TenantId == tenantId && candidateCodes.Contains(b.BranchCode))
                .ToDictionaryAsync(b => b.BranchCode, b => b.BranchId, StringComparer.OrdinalIgnoreCase, cancellationToken);

            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                var code = SeedSlug.From($"{item.StateCode}-{item.City}");
                var organizationId = organizationIds[i % organizationIds.Count];

                if (existingByCode.TryGetValue(code, out var existingId))
                {
                    context.Registry.RegisterBranch(tenantId, code, existingId);
                    continue;
                }

                var branch = BranchBuilder.Build(tenantId, organizationId, item);

                context.DbContext.Branches.Add(new EfBranch
                {
                    BranchId = branch.Id,
                    TenantId = tenantId,
                    OrganizationId = organizationId,
                    BranchCode = branch.Code,
                    BranchName = branch.Name,
                    StateCode = branch.StateCode,
                    City = item.City,
                    AddressLine1 = item.AddressLine1,
                    AddressLine2 = item.AddressLine2,
                    PostalCode = item.PostalCode,
                    CountryCode = branch.CountryCode,
                    TimeZone = branch.TimeZoneId!,
                    status = branch.Status
                });

                context.Registry.RegisterBranch(tenantId, code, branch.Id);
            }
        }

        await context.DbContext.SaveChangesAsync(cancellationToken);
    }
}
