using NotaryPlatform.Domain.Features.Journal.Aggregates;

namespace NotaryPlatform.Domain.Features.Journal.Repositories;

public interface IJournalEntryIdDocumentRepository
{
    Task<JournalEntryIdDocument?> GetByIdAsync(Guid journalEntryIdDocumentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntryIdDocument>> ListByJournalEntryAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
    Task AddAsync(JournalEntryIdDocument document, CancellationToken cancellationToken = default);
    Task UpdateAsync(JournalEntryIdDocument document, CancellationToken cancellationToken = default);
    Task DeleteAsync(JournalEntryIdDocument document, CancellationToken cancellationToken = default);
}
