using NotaryPlatform.Domain.Features.Journal.Aggregates;

namespace NotaryPlatform.Domain.Features.Journal.Repositories;

public interface IJournalEntrySignerRepository
{
    Task<JournalEntrySigner?> GetByIdAsync(Guid journalEntrySignerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntrySigner>> ListByJournalEntryAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
    Task AddAsync(JournalEntrySigner signer, CancellationToken cancellationToken = default);
    Task UpdateAsync(JournalEntrySigner signer, CancellationToken cancellationToken = default);
    Task DeleteAsync(JournalEntrySigner signer, CancellationToken cancellationToken = default);
}
