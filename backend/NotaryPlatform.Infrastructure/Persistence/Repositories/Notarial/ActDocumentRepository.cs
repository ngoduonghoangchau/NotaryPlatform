using NotaryPlatform.Domain.Features.Notarial.Aggregates;
using NotaryPlatform.Domain.Features.Notarial.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfActDocument = NotaryPlatform.Infrastructure.Persistence.Generated.Notarial.ActDocument;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Notarial;

public sealed class ActDocumentRepository : RepositoryBase<EfActDocument, ActDocument>, IActDocumentRepository
{
    public ActDocumentRepository(NotaryPlatformDbContext context) : base(context) { }
}
