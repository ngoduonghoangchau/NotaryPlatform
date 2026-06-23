using NotaryPlatform.Domain.Features.Identity.Aggregates;
using NotaryPlatform.Domain.Features.Identity.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfNotaryBond = NotaryPlatform.Infrastructure.Persistence.Generated.Identity.NotaryBond;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Identity;

public sealed class NotaryBondRepository : RepositoryBase<EfNotaryBond, NotaryBond>, INotaryBondRepository
{
    public NotaryBondRepository(NotaryPlatformDbContext context) : base(context) { }
}
