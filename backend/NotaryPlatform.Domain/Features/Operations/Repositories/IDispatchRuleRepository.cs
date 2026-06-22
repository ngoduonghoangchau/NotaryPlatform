using NotaryPlatform.Domain.Features.Operations.Aggregates;

namespace NotaryPlatform.Domain.Features.Operations.Repositories;

public interface IDispatchRuleRepository
{
    Task<DispatchRule?> GetByIdAsync(Guid dispatchRuleId, CancellationToken cancellationToken = default);
    Task<DispatchRule?> GetByCodeAsync(Guid tenantId, string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DispatchRule>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(DispatchRule dispatchRule, CancellationToken cancellationToken = default);
    Task UpdateAsync(DispatchRule dispatchRule, CancellationToken cancellationToken = default);
    Task DeleteAsync(DispatchRule dispatchRule, CancellationToken cancellationToken = default);
}
