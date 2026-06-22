using NotaryPlatform.Domain.Features.Journal.Aggregates;

namespace NotaryPlatform.Domain.Features.Journal.Repositories;

public interface IJournalAuditLogRepository
{
    Task<JournalAuditLog?> GetByIdAsync(Guid journalAuditLogId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalAuditLog>> ListByJournalEntryAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
    Task AddAsync(JournalAuditLog auditLog, CancellationToken cancellationToken = default);
    Task DeleteAsync(JournalAuditLog auditLog, CancellationToken cancellationToken = default);
}
