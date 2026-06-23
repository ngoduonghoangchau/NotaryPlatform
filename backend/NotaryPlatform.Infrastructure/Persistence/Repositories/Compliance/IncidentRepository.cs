using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfIncident = NotaryPlatform.Infrastructure.Persistence.Generated.Compliance.Incident;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Compliance;

public sealed class IncidentRepository : RepositoryBase<EfIncident, Incident>, IIncidentRepository
{
    public IncidentRepository(NotaryPlatformDbContext context) : base(context) { }
}
