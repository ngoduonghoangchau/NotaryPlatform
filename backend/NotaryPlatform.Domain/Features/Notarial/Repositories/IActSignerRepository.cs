using NotaryPlatform.Domain.Features.Notarial.Aggregates;

namespace NotaryPlatform.Domain.Features.Notarial.Repositories;

public interface IActSignerRepository
{
    Task<ActSigner?> GetByIdAsync(Guid actSignerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ActSigner>> ListByActIdAsync(Guid notarialActId, CancellationToken cancellationToken = default);
    Task AddAsync(ActSigner signer, CancellationToken cancellationToken = default);
    Task UpdateAsync(ActSigner signer, CancellationToken cancellationToken = default);
    Task DeleteAsync(ActSigner signer, CancellationToken cancellationToken = default);
}
