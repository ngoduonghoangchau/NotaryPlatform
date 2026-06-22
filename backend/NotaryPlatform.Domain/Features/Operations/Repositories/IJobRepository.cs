using NotaryPlatform.Domain.Features.Operations.Aggregates;
using NotaryPlatform.Domain.Features.Operations.ValueObjects;

namespace NotaryPlatform.Domain.Features.Operations.Repositories;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<Job?> GetByCodeAsync(JobCode jobCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Job>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Job>> ListByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(Job job, CancellationToken cancellationToken = default);
    Task UpdateAsync(Job job, CancellationToken cancellationToken = default);
    Task DeleteAsync(Job job, CancellationToken cancellationToken = default);
}
