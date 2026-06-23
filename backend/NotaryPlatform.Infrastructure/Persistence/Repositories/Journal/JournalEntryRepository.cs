using NotaryPlatform.Domain.Features.Journal.Aggregates;
using NotaryPlatform.Domain.Features.Journal.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJournalEntry = NotaryPlatform.Infrastructure.Persistence.Generated.Journal.JournalEntry;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Journal;

public sealed class JournalEntryRepository : RepositoryBase<EfJournalEntry, JournalEntry>, IJournalEntryRepository
{
    public JournalEntryRepository(NotaryPlatformDbContext context) : base(context) { }
}
