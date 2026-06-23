using NotaryPlatform.Domain.Features.Communication.Aggregates;
using NotaryPlatform.Domain.Features.Communication.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCommunicationMessage = NotaryPlatform.Infrastructure.Persistence.Generated.Communication.CommunicationMessage;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Communication;

public sealed class CommunicationMessageRepository : RepositoryBase<EfCommunicationMessage, CommunicationMessage>, ICommunicationMessageRepository
{
    public CommunicationMessageRepository(NotaryPlatformDbContext context) : base(context) { }
}
