using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfIncidentAction = NotaryPlatform.Infrastructure.Persistence.Generated.Compliance.IncidentAction;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Compliance;

public sealed class IncidentActionRepository : RepositoryBase<EfIncidentAction, IncidentAction>, IIncidentActionRepository
{
    public IncidentActionRepository(NotaryPlatformDbContext context) : base(context) { }
}
