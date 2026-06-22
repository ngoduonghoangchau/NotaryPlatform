using NotaryPlatform.Domain.Features.Journal.Aggregates;
using NotaryPlatform.Domain.Features.Journal.Enums;

namespace NotaryPlatform.Domain.Features.Journal.Repositories;

public interface IJournalExportRepository
{
    Task<JournalExport?> GetByIdAsync(Guid journalExportId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalExport>> ListByJournalEntryAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalExport>> ListByStatusAsync(JournalExportStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(JournalExport export, CancellationToken cancellationToken = default);
    Task UpdateAsync(JournalExport export, CancellationToken cancellationToken = default);
    Task DeleteAsync(JournalExport export, CancellationToken cancellationToken = default);
}
