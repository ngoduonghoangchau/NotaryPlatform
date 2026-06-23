using NotaryPlatform.Domain.Features.Journal.Aggregates;
using NotaryPlatform.Domain.Features.Journal.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJournalEntryLink = NotaryPlatform.Infrastructure.Persistence.Generated.Journal.JournalEntryLink;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Journal;

public sealed class JournalEntryLinkRepository : RepositoryBase<EfJournalEntryLink, JournalEntryLink>, IJournalEntryLinkRepository
{
    public JournalEntryLinkRepository(NotaryPlatformDbContext context) : base(context) { }
}
