using NotaryPlatform.Domain.Features.Security.Aggregates;

namespace NotaryPlatform.Domain.Features.Security.Repositories;

public interface IEmergencyLockRepository
{
    Task<EmergencyLock?> GetByIdAsync(Guid emergencyLockId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmergencyLock>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmergencyLock>> ListActiveByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(EmergencyLock emergencyLock, CancellationToken cancellationToken = default);
    Task UpdateAsync(EmergencyLock emergencyLock, CancellationToken cancellationToken = default);
    Task DeleteAsync(EmergencyLock emergencyLock, CancellationToken cancellationToken = default);
}
