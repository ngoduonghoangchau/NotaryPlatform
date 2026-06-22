using NotaryPlatform.Domain.Features.Billing.Aggregates;

namespace NotaryPlatform.Domain.Features.Billing.Repositories;

public interface INotaryCommissionPayableRepository
{
    Task<NotaryCommissionPayable?> GetByIdAsync(Guid notaryCommissionPayableId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotaryCommissionPayable>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotaryCommissionPayable>> ListByNotaryAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task AddAsync(NotaryCommissionPayable payable, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotaryCommissionPayable payable, CancellationToken cancellationToken = default);
    Task DeleteAsync(NotaryCommissionPayable payable, CancellationToken cancellationToken = default);
}
