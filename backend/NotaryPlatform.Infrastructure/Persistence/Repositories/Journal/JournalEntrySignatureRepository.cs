using NotaryPlatform.Domain.Features.Journal.Aggregates;
using NotaryPlatform.Domain.Features.Journal.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJournalEntrySignature = NotaryPlatform.Infrastructure.Persistence.Generated.Journal.JournalEntrySignature;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Journal;

public sealed class JournalEntrySignatureRepository : RepositoryBase<EfJournalEntrySignature, JournalEntrySignature>, IJournalEntrySignatureRepository
{
    public JournalEntrySignatureRepository(NotaryPlatformDbContext context) : base(context) { }
}
