using NotaryPlatform.Domain.Features.Identity.Aggregates;

namespace NotaryPlatform.Domain.Features.Identity.Repositories;

public interface INotaryLicenseRepository
{
    Task<NotaryLicense?> GetByIdAsync(Guid notaryLicenseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotaryLicense>> ListByNotaryIdAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task AddAsync(NotaryLicense license, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotaryLicense license, CancellationToken cancellationToken = default);
    Task DeleteAsync(NotaryLicense license, CancellationToken cancellationToken = default);
}
