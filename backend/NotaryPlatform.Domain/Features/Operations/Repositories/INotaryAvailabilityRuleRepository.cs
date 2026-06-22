using NotaryPlatform.Domain.Features.Operations.Aggregates;

namespace NotaryPlatform.Domain.Features.Operations.Repositories;

public interface INotaryAvailabilityRuleRepository
{
    Task<NotaryAvailabilityRule?> GetByIdAsync(Guid notaryAvailabilityRuleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotaryAvailabilityRule>> ListByNotaryIdAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task AddAsync(NotaryAvailabilityRule rule, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotaryAvailabilityRule rule, CancellationToken cancellationToken = default);
    Task DeleteAsync(NotaryAvailabilityRule rule, CancellationToken cancellationToken = default);
}
