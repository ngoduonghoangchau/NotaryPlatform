using NotaryPlatform.Domain.Features.CRM.Aggregates;

namespace NotaryPlatform.Domain.Features.CRM.Repositories;

public interface ICustomerSegmentRepository
{
    Task<CustomerSegment?> GetByIdAsync(Guid customerSegmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerSegment>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(CustomerSegment segment, CancellationToken cancellationToken = default);
    Task UpdateAsync(CustomerSegment segment, CancellationToken cancellationToken = default);
    Task DeleteAsync(CustomerSegment segment, CancellationToken cancellationToken = default);
}
