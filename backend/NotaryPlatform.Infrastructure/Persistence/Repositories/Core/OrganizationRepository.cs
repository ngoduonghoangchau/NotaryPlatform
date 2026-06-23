using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Domain.Features.Core.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfOrganization = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Organization;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Core;

public sealed class OrganizationRepository : RepositoryBase<EfOrganization, Organization>, IOrganizationRepository
{
    public OrganizationRepository(NotaryPlatformDbContext context) : base(context) { }
}
