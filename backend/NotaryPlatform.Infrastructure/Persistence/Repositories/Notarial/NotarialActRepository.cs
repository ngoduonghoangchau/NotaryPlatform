using NotaryPlatform.Domain.Features.Notarial.Aggregates;
using NotaryPlatform.Domain.Features.Notarial.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfNotarialAct = NotaryPlatform.Infrastructure.Persistence.Generated.Notarial.NotarialAct;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Notarial;

public sealed class NotarialActRepository : RepositoryBase<EfNotarialAct, NotarialAct>, INotarialActRepository
{
    public NotarialActRepository(NotaryPlatformDbContext context) : base(context) { }
}
