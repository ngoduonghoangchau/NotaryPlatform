using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.ValueObjects;

namespace NotaryPlatform.Domain.Features.Compliance.Repositories;

public interface ILegalHoldRepository
{
    Task<LegalHold?> GetByIdAsync(Guid legalHoldId, CancellationToken cancellationToken = default);
    Task<LegalHold?> GetByCodeAsync(HoldCode holdCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LegalHold>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(LegalHold legalHold, CancellationToken cancellationToken = default);
    Task UpdateAsync(LegalHold legalHold, CancellationToken cancellationToken = default);
    Task DeleteAsync(LegalHold legalHold, CancellationToken cancellationToken = default);
}
