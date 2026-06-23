using NotaryPlatform.Domain.Features.Notarial.Aggregates;
using NotaryPlatform.Domain.Features.Notarial.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfActStatusHistory = NotaryPlatform.Infrastructure.Persistence.Generated.Notarial.ActStatusHistory;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Notarial;

public sealed class ActStatusHistoryRepository : RepositoryBase<EfActStatusHistory, ActStatusHistory>, IActStatusHistoryRepository
{
    public ActStatusHistoryRepository(NotaryPlatformDbContext context) : base(context) { }
}
