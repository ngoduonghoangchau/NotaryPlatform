using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfSecurityIncident = NotaryPlatform.Infrastructure.Persistence.Generated.Security.SecurityIncident;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Security;

public sealed class SecurityIncidentRepository : RepositoryBase<EfSecurityIncident, SecurityIncident>, ISecurityIncidentRepository
{
    public SecurityIncidentRepository(NotaryPlatformDbContext context) : base(context) { }
}
