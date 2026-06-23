using NotaryPlatform.Domain.Features.Communication.Aggregates;
using NotaryPlatform.Domain.Features.Communication.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCommunicationParticipant = NotaryPlatform.Infrastructure.Persistence.Generated.Communication.CommunicationParticipant;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Communication;

public sealed class CommunicationParticipantRepository : RepositoryBase<EfCommunicationParticipant, CommunicationParticipant>, ICommunicationParticipantRepository
{
    public CommunicationParticipantRepository(NotaryPlatformDbContext context) : base(context) { }
}
