using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Domain.Features.Core.ValueObjects;

namespace NotaryPlatform.Domain.Features.Core.Repositories;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Tenant?> GetByCodeAsync(TenantCode code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tenant>> ListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task DeleteAsync(Tenant tenant, CancellationToken cancellationToken = default);
}
