using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Domain.Features.Core.ValueObjects;

namespace NotaryPlatform.Domain.Features.Core.Repositories;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<Organization?> GetByCodeAsync(OrganizationCode code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Organization>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Organization organization, CancellationToken cancellationToken = default);
    Task UpdateAsync(Organization organization, CancellationToken cancellationToken = default);
    Task DeleteAsync(Organization organization, CancellationToken cancellationToken = default);
}
