using NotaryPlatform.Domain.Features.Billing.Aggregates;

namespace NotaryPlatform.Domain.Features.Billing.Repositories;

public interface IAccountsReceivableSnapshotRepository
{
    Task<AccountsReceivableSnapshot?> GetByIdAsync(Guid accountsReceivableSnapshotId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountsReceivableSnapshot>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountsReceivableSnapshot>> ListByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(AccountsReceivableSnapshot snapshot, CancellationToken cancellationToken = default);
    Task DeleteAsync(AccountsReceivableSnapshot snapshot, CancellationToken cancellationToken = default);
}
