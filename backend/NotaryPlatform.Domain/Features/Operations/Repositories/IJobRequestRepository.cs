using NotaryPlatform.Domain.Features.Operations.Aggregates;
using NotaryPlatform.Domain.Features.Operations.ValueObjects;

namespace NotaryPlatform.Domain.Features.Operations.Repositories;

public interface IJobRequestRepository
{
    Task<JobRequest?> GetByIdAsync(Guid jobRequestId, CancellationToken cancellationToken = default);
    Task<JobRequest?> GetByCodeAsync(JobCode jobCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobRequest>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobRequest>> ListByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(JobRequest jobRequest, CancellationToken cancellationToken = default);
    Task UpdateAsync(JobRequest jobRequest, CancellationToken cancellationToken = default);
    Task DeleteAsync(JobRequest jobRequest, CancellationToken cancellationToken = default);
}
