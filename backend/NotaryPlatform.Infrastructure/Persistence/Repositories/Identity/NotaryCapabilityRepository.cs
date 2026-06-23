using NotaryPlatform.Domain.Features.Identity.Aggregates;
using NotaryPlatform.Domain.Features.Identity.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfNotaryCapability = NotaryPlatform.Infrastructure.Persistence.Generated.Identity.NotaryCapability;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Identity;

public sealed class NotaryCapabilityRepository : RepositoryBase<EfNotaryCapability, NotaryCapability>, INotaryCapabilityRepository
{
    public NotaryCapabilityRepository(NotaryPlatformDbContext context) : base(context) { }
}
