using NotaryPlatform.Domain.Features.Operations.Aggregates;

namespace NotaryPlatform.Domain.Features.Operations.Repositories;

public interface IServiceTypeRepository
{
    Task<ServiceType?> GetByIdAsync(Guid serviceTypeId, CancellationToken cancellationToken = default);
    Task<ServiceType?> GetByCodeAsync(Guid tenantId, string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ServiceType>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(ServiceType serviceType, CancellationToken cancellationToken = default);
    Task UpdateAsync(ServiceType serviceType, CancellationToken cancellationToken = default);
    Task DeleteAsync(ServiceType serviceType, CancellationToken cancellationToken = default);
}
