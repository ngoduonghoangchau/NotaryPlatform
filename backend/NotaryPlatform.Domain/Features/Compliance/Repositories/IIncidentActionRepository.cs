using NotaryPlatform.Domain.Features.Compliance.Aggregates;

namespace NotaryPlatform.Domain.Features.Compliance.Repositories;

public interface IIncidentActionRepository
{
    Task<IncidentAction?> GetByIdAsync(Guid incidentActionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IncidentAction>> ListByIncidentAsync(Guid incidentId, CancellationToken cancellationToken = default);
    Task AddAsync(IncidentAction action, CancellationToken cancellationToken = default);
    Task UpdateAsync(IncidentAction action, CancellationToken cancellationToken = default);
    Task DeleteAsync(IncidentAction action, CancellationToken cancellationToken = default);
}
