using NotaryPlatform.Domain.Features.Operations.Aggregates;
using NotaryPlatform.Domain.Features.Operations.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfScheduleBlock = NotaryPlatform.Infrastructure.Persistence.Generated.Operations.ScheduleBlock;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Operations;

public sealed class ScheduleBlockRepository : RepositoryBase<EfScheduleBlock, ScheduleBlock>, IScheduleBlockRepository
{
    public ScheduleBlockRepository(NotaryPlatformDbContext context) : base(context) { }
}
