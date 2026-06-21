using NotaryPlatform.Domain.Features.Identity.Aggregates;

namespace NotaryPlatform.Domain.Features.Identity.Repositories;

public interface INotaryCommissionRepository
{
    Task<NotaryCommission?> GetByIdAsync(Guid notaryCommissionId, CancellationToken cancellationToken = default);
    Task<NotaryCommission?> GetCurrentByNotaryIdAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotaryCommission>> ListByNotaryIdAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task AddAsync(NotaryCommission commission, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotaryCommission commission, CancellationToken cancellationToken = default);
    Task DeleteAsync(NotaryCommission commission, CancellationToken cancellationToken = default);
}
