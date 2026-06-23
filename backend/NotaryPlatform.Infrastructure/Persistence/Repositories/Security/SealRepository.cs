using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfSeal = NotaryPlatform.Infrastructure.Persistence.Generated.Security.Seal;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Security;

public sealed class SealRepository : RepositoryBase<EfSeal, Seal>, ISealRepository
{
    public SealRepository(NotaryPlatformDbContext context) : base(context) { }
}
