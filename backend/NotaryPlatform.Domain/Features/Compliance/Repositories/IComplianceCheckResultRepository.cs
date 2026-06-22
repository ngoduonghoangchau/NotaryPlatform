using NotaryPlatform.Domain.Features.Compliance.Aggregates;

namespace NotaryPlatform.Domain.Features.Compliance.Repositories;

public interface IComplianceCheckResultRepository
{
    Task<ComplianceCheckResult?> GetByIdAsync(Guid complianceCheckResultId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ComplianceCheckResult>> ListByCheckAsync(Guid complianceCheckId, CancellationToken cancellationToken = default);
    Task AddAsync(ComplianceCheckResult result, CancellationToken cancellationToken = default);
    Task DeleteAsync(ComplianceCheckResult result, CancellationToken cancellationToken = default);
}
