using NotaryPlatform.Domain.Features.Journal.Aggregates;
using NotaryPlatform.Domain.Features.Journal.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJournalExport = NotaryPlatform.Infrastructure.Persistence.Generated.Journal.JournalExport;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Journal;

public sealed class JournalExportRepository : RepositoryBase<EfJournalExport, JournalExport>, IJournalExportRepository
{
    public JournalExportRepository(NotaryPlatformDbContext context) : base(context) { }
}
