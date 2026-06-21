using NotaryPlatform.Domain.Features.Identity.Aggregates;
using NotaryPlatform.Domain.Features.Identity.Enums;

namespace NotaryPlatform.Domain.Features.Identity.Repositories;

public interface INotaryCapabilityRepository
{
    Task<NotaryCapability?> GetByIdAsync(Guid notaryCapabilityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotaryCapability>> ListByNotaryIdAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotaryCapability>> ListByCapabilityAsync(CapabilityCode capability, CancellationToken cancellationToken = default);
    Task AddAsync(NotaryCapability capability, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotaryCapability capability, CancellationToken cancellationToken = default);
    Task DeleteAsync(NotaryCapability capability, CancellationToken cancellationToken = default);
}
