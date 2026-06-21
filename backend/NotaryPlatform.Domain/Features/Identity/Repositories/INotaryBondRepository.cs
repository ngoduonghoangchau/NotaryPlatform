using NotaryPlatform.Domain.Features.Identity.Aggregates;

namespace NotaryPlatform.Domain.Features.Identity.Repositories;

public interface INotaryBondRepository
{
    Task<NotaryBond?> GetByIdAsync(Guid notaryBondId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotaryBond>> ListByNotaryIdAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task AddAsync(NotaryBond bond, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotaryBond bond, CancellationToken cancellationToken = default);
    Task DeleteAsync(NotaryBond bond, CancellationToken cancellationToken = default);
}
