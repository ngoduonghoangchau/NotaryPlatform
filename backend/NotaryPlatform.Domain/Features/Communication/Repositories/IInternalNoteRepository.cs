using NotaryPlatform.Domain.Features.Communication.Aggregates;

namespace NotaryPlatform.Domain.Features.Communication.Repositories;

public interface IInternalNoteRepository
{
    Task<InternalNote?> GetByIdAsync(Guid internalNoteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InternalNote>> ListByThreadAsync(Guid communicationThreadId, CancellationToken cancellationToken = default);
    Task AddAsync(InternalNote note, CancellationToken cancellationToken = default);
    Task UpdateAsync(InternalNote note, CancellationToken cancellationToken = default);
    Task DeleteAsync(InternalNote note, CancellationToken cancellationToken = default);
}
