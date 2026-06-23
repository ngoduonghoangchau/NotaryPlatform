using NotaryPlatform.Domain.Features.Communication.Aggregates;
using NotaryPlatform.Domain.Features.Communication.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCommunicationAttachment = NotaryPlatform.Infrastructure.Persistence.Generated.Communication.CommunicationAttachment;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Communication;

public sealed class CommunicationAttachmentRepository : RepositoryBase<EfCommunicationAttachment, CommunicationAttachment>, ICommunicationAttachmentRepository
{
    public CommunicationAttachmentRepository(NotaryPlatformDbContext context) : base(context) { }
}
