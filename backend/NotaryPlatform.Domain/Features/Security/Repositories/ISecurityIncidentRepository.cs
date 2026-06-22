using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Domain.Features.Security.Repositories;

public interface ISecurityIncidentRepository
{
    Task<SecurityIncident?> GetByIdAsync(Guid securityIncidentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SecurityIncident>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SecurityIncident>> ListByStatusAsync(IncidentStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(SecurityIncident incident, CancellationToken cancellationToken = default);
    Task UpdateAsync(SecurityIncident incident, CancellationToken cancellationToken = default);
    Task DeleteAsync(SecurityIncident incident, CancellationToken cancellationToken = default);
}
