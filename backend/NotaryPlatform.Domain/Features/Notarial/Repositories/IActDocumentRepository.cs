using NotaryPlatform.Domain.Features.Notarial.Aggregates;

namespace NotaryPlatform.Domain.Features.Notarial.Repositories;

public interface IActDocumentRepository
{
    Task<ActDocument?> GetByIdAsync(Guid actDocumentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ActDocument>> ListByActIdAsync(Guid notarialActId, CancellationToken cancellationToken = default);
    Task AddAsync(ActDocument document, CancellationToken cancellationToken = default);
    Task UpdateAsync(ActDocument document, CancellationToken cancellationToken = default);
    Task DeleteAsync(ActDocument document, CancellationToken cancellationToken = default);
}
