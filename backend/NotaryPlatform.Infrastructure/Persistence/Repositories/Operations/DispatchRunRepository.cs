using NotaryPlatform.Domain.Features.Operations.Aggregates;
using NotaryPlatform.Domain.Features.Operations.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfDispatchRun = NotaryPlatform.Infrastructure.Persistence.Generated.Operations.DispatchRun;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Operations;

public sealed class DispatchRunRepository : RepositoryBase<EfDispatchRun, DispatchRun>, IDispatchRunRepository
{
    public DispatchRunRepository(NotaryPlatformDbContext context) : base(context) { }
}
