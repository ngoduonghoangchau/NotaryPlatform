using NotaryPlatform.Domain.Features.Identity.Aggregates;
using NotaryPlatform.Domain.Features.Identity.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfNotary = NotaryPlatform.Infrastructure.Persistence.Generated.Identity.Notary;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Identity;

public sealed class NotaryRepository : RepositoryBase<EfNotary, Notary>, INotaryRepository
{
    public NotaryRepository(NotaryPlatformDbContext context) : base(context) { }
}
