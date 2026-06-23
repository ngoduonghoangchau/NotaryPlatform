using NotaryPlatform.Domain.Features.Communication.Aggregates;
using NotaryPlatform.Domain.Features.Communication.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCallLog = NotaryPlatform.Infrastructure.Persistence.Generated.Communication.CallLog;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Communication;

public sealed class CallLogRepository : RepositoryBase<EfCallLog, CallLog>, ICallLogRepository
{
    public CallLogRepository(NotaryPlatformDbContext context) : base(context) { }
}
