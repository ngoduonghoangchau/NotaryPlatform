using NotaryPlatform.Domain.Features.Operations.Aggregates;

namespace NotaryPlatform.Domain.Features.Operations.Repositories;

public interface IDispatchRunRepository
{
    Task<DispatchRun?> GetByIdAsync(Guid dispatchRunId, CancellationToken cancellationToken = default);
    Task<DispatchRun?> GetByCodeAsync(Guid tenantId, string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DispatchRun>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(DispatchRun dispatchRun, CancellationToken cancellationToken = default);
    Task UpdateAsync(DispatchRun dispatchRun, CancellationToken cancellationToken = default);
    Task DeleteAsync(DispatchRun dispatchRun, CancellationToken cancellationToken = default);
}
