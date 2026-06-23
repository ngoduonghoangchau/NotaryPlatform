using NotaryPlatform.Domain.Features.Journal.Aggregates;
using NotaryPlatform.Domain.Features.Journal.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJournalEntrySigner = NotaryPlatform.Infrastructure.Persistence.Generated.Journal.JournalEntrySigner;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Journal;

public sealed class JournalEntrySignerRepository : RepositoryBase<EfJournalEntrySigner, JournalEntrySigner>, IJournalEntrySignerRepository
{
    public JournalEntrySignerRepository(NotaryPlatformDbContext context) : base(context) { }
}
