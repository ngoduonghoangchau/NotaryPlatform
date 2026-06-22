using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.ValueObjects;

namespace NotaryPlatform.Domain.Features.Compliance.Repositories;

public interface IIncidentRepository
{
    Task<Incident?> GetByIdAsync(Guid incidentId, CancellationToken cancellationToken = default);
    Task<Incident?> GetByCodeAsync(IncidentCode incidentCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Incident>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Incident>> ListByStatusAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Incident incident, CancellationToken cancellationToken = default);
    Task UpdateAsync(Incident incident, CancellationToken cancellationToken = default);
    Task DeleteAsync(Incident incident, CancellationToken cancellationToken = default);
}
