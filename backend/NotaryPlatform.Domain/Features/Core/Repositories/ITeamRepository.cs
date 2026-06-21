using NotaryPlatform.Domain.Features.Core.Aggregates;

namespace NotaryPlatform.Domain.Features.Core.Repositories;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid teamId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Team>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Team>> ListByBranchAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task AddAsync(Team team, CancellationToken cancellationToken = default);
    Task UpdateAsync(Team team, CancellationToken cancellationToken = default);
    Task DeleteAsync(Team team, CancellationToken cancellationToken = default);
}
