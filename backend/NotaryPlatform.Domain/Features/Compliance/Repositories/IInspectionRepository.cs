using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.ValueObjects;

namespace NotaryPlatform.Domain.Features.Compliance.Repositories;

public interface IInspectionRepository
{
    Task<Inspection?> GetByIdAsync(Guid inspectionId, CancellationToken cancellationToken = default);
    Task<Inspection?> GetByCodeAsync(IncidentCode inspectionCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Inspection>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Inspection inspection, CancellationToken cancellationToken = default);
    Task UpdateAsync(Inspection inspection, CancellationToken cancellationToken = default);
    Task DeleteAsync(Inspection inspection, CancellationToken cancellationToken = default);
}
