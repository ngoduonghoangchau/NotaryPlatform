using NotaryPlatform.Domain.Features.Journal.Aggregates;

namespace NotaryPlatform.Domain.Features.Journal.Repositories;

public interface IJournalEntrySignatureRepository
{
    Task<JournalEntrySignature?> GetByIdAsync(Guid journalEntrySignatureId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntrySignature>> ListByJournalEntryAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
    Task AddAsync(JournalEntrySignature signature, CancellationToken cancellationToken = default);
    Task UpdateAsync(JournalEntrySignature signature, CancellationToken cancellationToken = default);
    Task DeleteAsync(JournalEntrySignature signature, CancellationToken cancellationToken = default);
}
