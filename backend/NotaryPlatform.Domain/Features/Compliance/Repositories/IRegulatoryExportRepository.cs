using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Domain.Features.Compliance.Repositories;

public interface IRegulatoryExportRepository
{
    Task<RegulatoryExport?> GetByIdAsync(Guid regulatoryExportId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegulatoryExport>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegulatoryExport>> ListByStatusAsync(ExportStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(RegulatoryExport regulatoryExport, CancellationToken cancellationToken = default);
    Task UpdateAsync(RegulatoryExport regulatoryExport, CancellationToken cancellationToken = default);
    Task DeleteAsync(RegulatoryExport regulatoryExport, CancellationToken cancellationToken = default);
}
