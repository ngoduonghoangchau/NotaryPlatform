using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Domain.Features.Core.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfTeam = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Team;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Core;

public sealed class TeamRepository : RepositoryBase<EfTeam, Team>, ITeamRepository
{
    public TeamRepository(NotaryPlatformDbContext context) : base(context) { }
}
