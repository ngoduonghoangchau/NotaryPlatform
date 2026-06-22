using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.ValueObjects;

namespace NotaryPlatform.Domain.Features.Compliance.Repositories;

public interface IComplianceRuleRepository
{
    Task<ComplianceRule?> GetByIdAsync(Guid complianceRuleId, CancellationToken cancellationToken = default);
    Task<ComplianceRule?> GetByCodeAsync(RuleCode ruleCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ComplianceRule>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(ComplianceRule rule, CancellationToken cancellationToken = default);
    Task UpdateAsync(ComplianceRule rule, CancellationToken cancellationToken = default);
    Task DeleteAsync(ComplianceRule rule, CancellationToken cancellationToken = default);
}
