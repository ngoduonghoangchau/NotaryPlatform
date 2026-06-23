using NotaryPlatform.Domain.Features.Journal.Aggregates;
using NotaryPlatform.Domain.Features.Journal.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJournalEntryIdDocument = NotaryPlatform.Infrastructure.Persistence.Generated.Journal.JournalEntryIdDocument;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Journal;

public sealed class JournalEntryIdDocumentRepository : RepositoryBase<EfJournalEntryIdDocument, JournalEntryIdDocument>, IJournalEntryIdDocumentRepository
{
    public JournalEntryIdDocumentRepository(NotaryPlatformDbContext context) : base(context) { }
}
