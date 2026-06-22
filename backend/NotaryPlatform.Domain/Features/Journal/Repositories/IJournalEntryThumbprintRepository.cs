using NotaryPlatform.Domain.Features.Journal.Aggregates;

namespace NotaryPlatform.Domain.Features.Journal.Repositories;

public interface IJournalEntryThumbprintRepository
{
    Task<JournalEntryThumbprint?> GetByIdAsync(Guid journalEntryThumbprintId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntryThumbprint>> ListByJournalEntryAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
    Task AddAsync(JournalEntryThumbprint thumbprint, CancellationToken cancellationToken = default);
    Task UpdateAsync(JournalEntryThumbprint thumbprint, CancellationToken cancellationToken = default);
    Task DeleteAsync(JournalEntryThumbprint thumbprint, CancellationToken cancellationToken = default);
}
