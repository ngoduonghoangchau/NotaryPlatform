using NotaryPlatform.Domain.Features.Notarial.Aggregates;
using NotaryPlatform.Domain.Features.Notarial.ValueObjects;

namespace NotaryPlatform.Domain.Features.Notarial.Repositories;

public interface INotarialActRepository
{
    Task<NotarialAct?> GetByIdAsync(Guid notarialActId, CancellationToken cancellationToken = default);
    Task<NotarialAct?> GetByCodeAsync(ActCode actCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotarialAct>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotarialAct>> ListByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(NotarialAct notarialAct, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotarialAct notarialAct, CancellationToken cancellationToken = default);
    Task DeleteAsync(NotarialAct notarialAct, CancellationToken cancellationToken = default);
}
