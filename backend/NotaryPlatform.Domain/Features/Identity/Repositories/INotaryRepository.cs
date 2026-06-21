using NotaryPlatform.Domain.Features.Identity.Aggregates;

namespace NotaryPlatform.Domain.Features.Identity.Repositories;

public interface INotaryRepository
{
    Task<Notary?> GetByIdAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task<Notary?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notary>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notary>> ListByBranchAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task AddAsync(Notary notary, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notary notary, CancellationToken cancellationToken = default);
    Task DeleteAsync(Notary notary, CancellationToken cancellationToken = default);
}
