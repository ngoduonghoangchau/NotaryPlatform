using NotaryPlatform.Domain.Features.Journal.Aggregates;
using NotaryPlatform.Domain.Features.Journal.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJournalTransferLog = NotaryPlatform.Infrastructure.Persistence.Generated.Journal.JournalTransferLog;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Journal;

public sealed class JournalTransferLogRepository : RepositoryBase<EfJournalTransferLog, JournalTransferLog>, IJournalTransferLogRepository
{
    public JournalTransferLogRepository(NotaryPlatformDbContext context) : base(context) { }
}
