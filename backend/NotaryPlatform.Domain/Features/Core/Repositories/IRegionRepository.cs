using NotaryPlatform.Domain.Features.Core.Aggregates;

namespace NotaryPlatform.Domain.Features.Core.Repositories;

public interface IRegionRepository
{
    Task<Region?> GetByIdAsync(Guid regionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Region>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Region>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task AddAsync(Region region, CancellationToken cancellationToken = default);
    Task UpdateAsync(Region region, CancellationToken cancellationToken = default);
    Task DeleteAsync(Region region, CancellationToken cancellationToken = default);
}
