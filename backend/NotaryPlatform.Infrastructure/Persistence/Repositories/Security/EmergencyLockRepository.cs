using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfEmergencyLock = NotaryPlatform.Infrastructure.Persistence.Generated.Security.EmergencyLock;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Security;

public sealed class EmergencyLockRepository : RepositoryBase<EfEmergencyLock, EmergencyLock>, IEmergencyLockRepository
{
    public EmergencyLockRepository(NotaryPlatformDbContext context) : base(context) { }
}
