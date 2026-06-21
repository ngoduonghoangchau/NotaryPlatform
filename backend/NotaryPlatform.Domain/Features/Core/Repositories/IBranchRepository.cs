using NotaryPlatform.Domain.Features.Core.Aggregates;

namespace NotaryPlatform.Domain.Features.Core.Repositories;

public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Branch>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Branch>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task AddAsync(Branch branch, CancellationToken cancellationToken = default);
    Task UpdateAsync(Branch branch, CancellationToken cancellationToken = default);
    Task DeleteAsync(Branch branch, CancellationToken cancellationToken = default);
}
