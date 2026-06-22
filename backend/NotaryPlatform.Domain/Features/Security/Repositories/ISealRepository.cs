using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.ValueObjects;

namespace NotaryPlatform.Domain.Features.Security.Repositories;

public interface ISealRepository
{
    Task<Seal?> GetByIdAsync(Guid sealId, CancellationToken cancellationToken = default);
    Task<Seal?> GetByCodeAsync(SealCode sealCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Seal>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Seal>> ListByNotaryAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task AddAsync(Seal seal, CancellationToken cancellationToken = default);
    Task UpdateAsync(Seal seal, CancellationToken cancellationToken = default);
    Task DeleteAsync(Seal seal, CancellationToken cancellationToken = default);
}
