using NotaryPlatform.Domain.Features.Notarial.Aggregates;

namespace NotaryPlatform.Domain.Features.Notarial.Repositories;

public interface IActStatusHistoryRepository
{
    Task<ActStatusHistory?> GetByIdAsync(Guid actStatusHistoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ActStatusHistory>> ListByActIdAsync(Guid notarialActId, CancellationToken cancellationToken = default);
    Task AddAsync(ActStatusHistory history, CancellationToken cancellationToken = default);
    Task DeleteAsync(ActStatusHistory history, CancellationToken cancellationToken = default);
}
