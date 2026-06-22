using NotaryPlatform.Domain.Features.Compliance.Aggregates;

namespace NotaryPlatform.Domain.Features.Compliance.Repositories;

public interface IRetentionPolicyRepository
{
    Task<RetentionPolicy?> GetByIdAsync(Guid retentionPolicyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RetentionPolicy>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(RetentionPolicy retentionPolicy, CancellationToken cancellationToken = default);
    Task UpdateAsync(RetentionPolicy retentionPolicy, CancellationToken cancellationToken = default);
    Task DeleteAsync(RetentionPolicy retentionPolicy, CancellationToken cancellationToken = default);
}
