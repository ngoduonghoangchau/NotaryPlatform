using NotaryPlatform.Domain.Features.Communication.Aggregates;
using NotaryPlatform.Domain.Features.Communication.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCommunicationThread = NotaryPlatform.Infrastructure.Persistence.Generated.Communication.CommunicationThread;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Communication;

public sealed class CommunicationThreadRepository : RepositoryBase<EfCommunicationThread, CommunicationThread>, ICommunicationThreadRepository
{
    public CommunicationThreadRepository(NotaryPlatformDbContext context) : base(context) { }
}
