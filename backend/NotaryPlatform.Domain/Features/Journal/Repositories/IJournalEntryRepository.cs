using NotaryPlatform.Domain.Features.Journal.Aggregates;
using NotaryPlatform.Domain.Features.Journal.ValueObjects;

namespace NotaryPlatform.Domain.Features.Journal.Repositories;

public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
    Task<JournalEntry?> GetByCodeAsync(JournalEntryCode journalEntryCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> ListByNotaryAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task AddAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
    Task UpdateAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
    Task DeleteAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
}
