using NotaryPlatform.Domain.Features.CRM.Aggregates;
using NotaryPlatform.Domain.Features.CRM.ValueObjects;

namespace NotaryPlatform.Domain.Features.CRM.Repositories;

public interface ISlaAgreementRepository
{
    Task<SlaAgreement?> GetByIdAsync(Guid slaAgreementId, CancellationToken cancellationToken = default);
    Task<SlaAgreement?> GetByCustomerAndCodeAsync(Guid customerId, SlaCode code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SlaAgreement>> ListByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(SlaAgreement slaAgreement, CancellationToken cancellationToken = default);
    Task UpdateAsync(SlaAgreement slaAgreement, CancellationToken cancellationToken = default);
    Task DeleteAsync(SlaAgreement slaAgreement, CancellationToken cancellationToken = default);
}
