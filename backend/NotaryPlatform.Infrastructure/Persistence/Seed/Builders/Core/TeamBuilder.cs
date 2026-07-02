using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;

/// <summary>
/// Builds a valid <see cref="Team"/> aggregate from fake seed data. The code
/// is prefixed with the owning branch's code so that every branch can have
/// its own "Mobile Signing Team" without colliding on <c>core.teams</c>'
/// tenant-wide unique code constraint.
/// <c>core.teams.branch_id</c> is <c>NOT NULL</c> at the database level, so a
/// branch is always required even though the domain aggregate models it as optional.
/// </summary>
public static class TeamBuilder
{
    public static Team Build(Guid tenantId, Guid organizationId, Guid branchId, string branchCode, Guid? regionId, TeamSeedData data)
    {
        // core.teams.team_code is StringLength(50); branch_code + team name can
        // exceed the default 30-char slug length, so use the full column width.
        var code = SeedSlug.From($"{branchCode}-{data.Name}", maxLength: 50);
        return Team.Create(tenantId, organizationId, code, data.Name, branchId, regionId);
    }
}
