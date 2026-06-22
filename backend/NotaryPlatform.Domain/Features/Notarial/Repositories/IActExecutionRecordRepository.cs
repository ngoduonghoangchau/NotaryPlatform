using NotaryPlatform.Domain.Features.Notarial.Aggregates;

namespace NotaryPlatform.Domain.Features.Notarial.Repositories;

public interface IActExecutionRecordRepository
{
    Task<ActExecutionRecord?> GetByIdAsync(Guid actExecutionRecordId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ActExecutionRecord>> ListByActIdAsync(Guid notarialActId, CancellationToken cancellationToken = default);
    Task AddAsync(ActExecutionRecord record, CancellationToken cancellationToken = default);
    Task UpdateAsync(ActExecutionRecord record, CancellationToken cancellationToken = default);
    Task DeleteAsync(ActExecutionRecord record, CancellationToken cancellationToken = default);
}
