using NotaryPlatform.Domain.Features.Journal.Aggregates;

namespace NotaryPlatform.Domain.Features.Journal.Repositories;

public interface IJournalEntryLinkRepository
{
    Task<JournalEntryLink?> GetByIdAsync(Guid journalEntryLinkId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntryLink>> ListByJournalEntryAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
    Task AddAsync(JournalEntryLink link, CancellationToken cancellationToken = default);
    Task UpdateAsync(JournalEntryLink link, CancellationToken cancellationToken = default);
    Task DeleteAsync(JournalEntryLink link, CancellationToken cancellationToken = default);
}
