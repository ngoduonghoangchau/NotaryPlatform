using NotaryPlatform.Domain.Features.Communication.Aggregates;
using NotaryPlatform.Domain.Features.Communication.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCommunicationDeliveryLog = NotaryPlatform.Infrastructure.Persistence.Generated.Communication.CommunicationDeliveryLog;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Communication;

public sealed class CommunicationDeliveryLogRepository : RepositoryBase<EfCommunicationDeliveryLog, CommunicationDeliveryLog>, ICommunicationDeliveryLogRepository
{
    public CommunicationDeliveryLogRepository(NotaryPlatformDbContext context) : base(context) { }
}
