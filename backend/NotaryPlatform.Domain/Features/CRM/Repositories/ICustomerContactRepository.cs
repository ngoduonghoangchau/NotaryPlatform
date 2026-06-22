using NotaryPlatform.Domain.Features.CRM.Aggregates;

namespace NotaryPlatform.Domain.Features.CRM.Repositories;

public interface ICustomerContactRepository
{
    Task<CustomerContact?> GetByIdAsync(Guid customerContactId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerContact>> ListByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(CustomerContact contact, CancellationToken cancellationToken = default);
    Task UpdateAsync(CustomerContact contact, CancellationToken cancellationToken = default);
    Task DeleteAsync(CustomerContact contact, CancellationToken cancellationToken = default);
}
