using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Domain.Features.Compliance.Repositories;

public interface IRetentionJobRepository
{
    Task<RetentionJob?> GetByIdAsync(Guid retentionJobId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RetentionJob>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RetentionJob>> ListByStatusAsync(ExportStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(RetentionJob retentionJob, CancellationToken cancellationToken = default);
    Task UpdateAsync(RetentionJob retentionJob, CancellationToken cancellationToken = default);
    Task DeleteAsync(RetentionJob retentionJob, CancellationToken cancellationToken = default);
}
