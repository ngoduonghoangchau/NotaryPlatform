using NotaryPlatform.Domain.Features.Billing.Aggregates;

namespace NotaryPlatform.Domain.Features.Billing.Repositories;

public interface ICreditRepository
{
    Task<Credit?> GetByIdAsync(Guid creditId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credit>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credit>> ListByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(Credit credit, CancellationToken cancellationToken = default);
    Task UpdateAsync(Credit credit, CancellationToken cancellationToken = default);
    Task DeleteAsync(Credit credit, CancellationToken cancellationToken = default);
}
