using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Domain.Features.Compliance.Repositories;

public interface IComplianceCheckRepository
{
    Task<ComplianceCheck?> GetByIdAsync(Guid complianceCheckId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ComplianceCheck>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ComplianceCheck>> ListByStatusAsync(CheckStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(ComplianceCheck check, CancellationToken cancellationToken = default);
    Task UpdateAsync(ComplianceCheck check, CancellationToken cancellationToken = default);
    Task DeleteAsync(ComplianceCheck check, CancellationToken cancellationToken = default);
}
