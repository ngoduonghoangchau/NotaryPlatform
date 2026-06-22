using NotaryPlatform.Domain.Features.Journal.Aggregates;

namespace NotaryPlatform.Domain.Features.Journal.Repositories;

public interface IJournalTransferLogRepository
{
    Task<JournalTransferLog?> GetByIdAsync(Guid journalTransferLogId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalTransferLog>> ListByJournalEntryAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
    Task AddAsync(JournalTransferLog transferLog, CancellationToken cancellationToken = default);
    Task DeleteAsync(JournalTransferLog transferLog, CancellationToken cancellationToken = default);
}
