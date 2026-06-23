using NotaryPlatform.Domain.Features.Journal.Aggregates;
using NotaryPlatform.Domain.Features.Journal.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJournalEntryThumbprint = NotaryPlatform.Infrastructure.Persistence.Generated.Journal.JournalEntryThumbprint;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Journal;

public sealed class JournalEntryThumbprintRepository : RepositoryBase<EfJournalEntryThumbprint, JournalEntryThumbprint>, IJournalEntryThumbprintRepository
{
    public JournalEntryThumbprintRepository(NotaryPlatformDbContext context) : base(context) { }
}
