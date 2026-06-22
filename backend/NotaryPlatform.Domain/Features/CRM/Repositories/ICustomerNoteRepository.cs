using NotaryPlatform.Domain.Features.CRM.Aggregates;

namespace NotaryPlatform.Domain.Features.CRM.Repositories;

public interface ICustomerNoteRepository
{
    Task<CustomerNote?> GetByIdAsync(Guid customerNoteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerNote>> ListByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(CustomerNote note, CancellationToken cancellationToken = default);
    Task UpdateAsync(CustomerNote note, CancellationToken cancellationToken = default);
    Task DeleteAsync(CustomerNote note, CancellationToken cancellationToken = default);
}
