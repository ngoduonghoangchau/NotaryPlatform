using NotaryPlatform.Domain.Features.CRM.Aggregates;

namespace NotaryPlatform.Domain.Features.CRM.Repositories;

public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(Guid contractId, CancellationToken cancellationToken = default);
    Task<Contract?> GetByCustomerAndNumberAsync(Guid customerId, string contractNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Contract>> ListByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(Contract contract, CancellationToken cancellationToken = default);
    Task UpdateAsync(Contract contract, CancellationToken cancellationToken = default);
    Task DeleteAsync(Contract contract, CancellationToken cancellationToken = default);
}
