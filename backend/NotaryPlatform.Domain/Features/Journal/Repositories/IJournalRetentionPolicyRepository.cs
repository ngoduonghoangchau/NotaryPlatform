using NotaryPlatform.Domain.Features.Journal.Aggregates;

namespace NotaryPlatform.Domain.Features.Journal.Repositories;

public interface IJournalRetentionPolicyRepository
{
    Task<JournalRetentionPolicy?> GetByIdAsync(Guid journalRetentionPolicyId, CancellationToken cancellationToken = default);
    Task<JournalRetentionPolicy?> GetByCodeAsync(Guid tenantId, string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalRetentionPolicy>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(JournalRetentionPolicy policy, CancellationToken cancellationToken = default);
    Task UpdateAsync(JournalRetentionPolicy policy, CancellationToken cancellationToken = default);
    Task DeleteAsync(JournalRetentionPolicy policy, CancellationToken cancellationToken = default);
}
