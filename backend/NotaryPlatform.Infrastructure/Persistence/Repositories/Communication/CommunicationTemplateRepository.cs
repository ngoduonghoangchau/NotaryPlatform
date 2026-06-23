using NotaryPlatform.Domain.Features.Communication.Aggregates;
using NotaryPlatform.Domain.Features.Communication.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCommunicationTemplate = NotaryPlatform.Infrastructure.Persistence.Generated.Communication.CommunicationTemplate;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Communication;

public sealed class CommunicationTemplateRepository : RepositoryBase<EfCommunicationTemplate, CommunicationTemplate>, ICommunicationTemplateRepository
{
    public CommunicationTemplateRepository(NotaryPlatformDbContext context) : base(context) { }
}
