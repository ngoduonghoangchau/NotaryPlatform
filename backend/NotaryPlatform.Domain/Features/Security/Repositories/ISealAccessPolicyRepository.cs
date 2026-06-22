using NotaryPlatform.Domain.Features.Security.Aggregates;

namespace NotaryPlatform.Domain.Features.Security.Repositories;

public interface ISealAccessPolicyRepository
{
    Task<SealAccessPolicy?> GetByIdAsync(Guid sealAccessPolicyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SealAccessPolicy>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(SealAccessPolicy policy, CancellationToken cancellationToken = default);
    Task UpdateAsync(SealAccessPolicy policy, CancellationToken cancellationToken = default);
    Task DeleteAsync(SealAccessPolicy policy, CancellationToken cancellationToken = default);
}
