using NotaryPlatform.Domain.Features.Communication.Aggregates;
using NotaryPlatform.Domain.Features.Communication.ValueObjects;

namespace NotaryPlatform.Domain.Features.Communication.Repositories;

public interface ICommunicationThreadRepository
{
    Task<CommunicationThread?> GetByIdAsync(Guid communicationThreadId, CancellationToken cancellationToken = default);
    Task<CommunicationThread?> GetByCodeAsync(ThreadCode threadCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommunicationThread>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommunicationThread>> ListByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(CommunicationThread communicationThread, CancellationToken cancellationToken = default);
    Task UpdateAsync(CommunicationThread communicationThread, CancellationToken cancellationToken = default);
    Task DeleteAsync(CommunicationThread communicationThread, CancellationToken cancellationToken = default);
}
