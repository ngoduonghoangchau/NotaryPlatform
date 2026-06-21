using NotaryPlatform.Domain.Features.Identity.Aggregates;

namespace NotaryPlatform.Domain.Features.Identity.Repositories;

public interface INotaryInsuranceRepository
{
    Task<NotaryInsurance?> GetByIdAsync(Guid notaryInsuranceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotaryInsurance>> ListByNotaryIdAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task AddAsync(NotaryInsurance insurance, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotaryInsurance insurance, CancellationToken cancellationToken = default);
    Task DeleteAsync(NotaryInsurance insurance, CancellationToken cancellationToken = default);
}
