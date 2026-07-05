using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;
using EfTeam = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Team;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Core;

/// <summary>Seeds <c>core.teams</c> — one realistic set of work teams per branch.</summary>
public sealed class TeamSeeder : ISeeder
{
    private readonly TeamDataGenerator _generator;

    public TeamSeeder(TeamDataGenerator generator) => _generator = generator;

    public string Name => "core.teams";

    public int Order => 600;

    public IReadOnlyCollection<SeedProfile> SupportedProfiles { get; } =
        [SeedProfile.Development, SeedProfile.Demo, SeedProfile.Staging];

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        var perBranch = context.VolumePlan.TeamsPerBranch;
        if (perBranch == 0)
        {
            return;
        }

        foreach (var tenantId in context.Registry.TenantIds)
        {
            var branches = await context.DbContext.Branches
                .AsNoTracking()
                .Where(b => b.TenantId == tenantId)
                .Select(b => new { b.BranchId, b.BranchCode, b.OrganizationId })
                .ToListAsync(cancellationToken);

            var regionRows = await context.DbContext.Regions
                .AsNoTracking()
                .Where(r => r.TenantId == tenantId)
                .Select(r => new { r.OrganizationId, r.RegionId })
                .ToListAsync(cancellationToken);

            // Grouped client-side to avoid relying on server-side GroupBy translation.
            var regionIdsByOrganization = regionRows
                .GroupBy(r => r.OrganizationId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.RegionId).ToList());

            foreach (var branch in branches)
            {
                var data = _generator.Generate(perBranch);
                var candidateCodes = data.Select(d => SeedSlug.From($"{branch.BranchCode}-{d.Name}")).ToList();

                var existingByCode = await context.DbContext.Teams
                    .AsNoTracking()
                    .Where(t => t.TenantId == tenantId && candidateCodes.Contains(t.TeamCode))
                    .ToDictionaryAsync(t => t.TeamCode, t => t.TeamId, StringComparer.OrdinalIgnoreCase, cancellationToken);

                regionIdsByOrganization.TryGetValue(branch.OrganizationId, out var regionIds);
                var regionId = regionIds is { Count: > 0 } ? regionIds[0] : (Guid?)null;

                foreach (var item in data)
                {
                    var code = SeedSlug.From($"{branch.BranchCode}-{item.Name}");

                    if (existingByCode.TryGetValue(code, out var existingId))
                    {
                        context.Registry.RegisterTeam(tenantId, code, existingId);
                        continue;
                    }

                    var team = TeamBuilder.Build(tenantId, branch.OrganizationId, branch.BranchId, branch.BranchCode, regionId, item);

                    context.DbContext.Teams.Add(new EfTeam
                    {
                        TeamId = team.Id,
                        TenantId = tenantId,
                        BranchId = branch.BranchId,
                        RegionId = team.RegionId,
                        TeamCode = team.Code,
                        TeamName = team.Name,
                        TeamType = item.TeamType,
                        status = team.Status
                    });

                    context.Registry.RegisterTeam(tenantId, code, team.Id);
                }
            }
        }

        await context.DbContext.SaveChangesAsync(cancellationToken);
    }
}
