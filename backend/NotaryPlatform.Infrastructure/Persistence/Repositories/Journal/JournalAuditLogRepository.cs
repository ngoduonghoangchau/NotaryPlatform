using NotaryPlatform.Domain.Features.Journal.Aggregates;
using NotaryPlatform.Domain.Features.Journal.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJournalAuditLog = NotaryPlatform.Infrastructure.Persistence.Generated.Journal.JournalAuditLog;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Journal;

public sealed class JournalAuditLogRepository : RepositoryBase<EfJournalAuditLog, JournalAuditLog>, IJournalAuditLogRepository
{
    public JournalAuditLogRepository(NotaryPlatformDbContext context) : base(context) { }
}
