using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.ValueObjects;

namespace NotaryPlatform.Domain.Features.Compliance.Repositories;

public interface IPolicyVersionRepository
{
    Task<PolicyVersion?> GetByIdAsync(Guid policyVersionId, CancellationToken cancellationToken = default);
    Task<PolicyVersion?> GetByCodeAsync(PolicyCode policyCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PolicyVersion>> ListByRuleAsync(Guid complianceRuleId, CancellationToken cancellationToken = default);
    Task AddAsync(PolicyVersion policyVersion, CancellationToken cancellationToken = default);
    Task UpdateAsync(PolicyVersion policyVersion, CancellationToken cancellationToken = default);
    Task DeleteAsync(PolicyVersion policyVersion, CancellationToken cancellationToken = default);
}
