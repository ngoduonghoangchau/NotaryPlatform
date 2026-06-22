using NotaryPlatform.Domain.Features.Communication.Aggregates;
using NotaryPlatform.Domain.Features.Communication.ValueObjects;

namespace NotaryPlatform.Domain.Features.Communication.Repositories;

public interface ICommunicationMessageRepository
{
    Task<CommunicationMessage?> GetByIdAsync(Guid communicationMessageId, CancellationToken cancellationToken = default);
    Task<CommunicationMessage?> GetByCodeAsync(MessageCode messageCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommunicationMessage>> ListByThreadAsync(Guid communicationThreadId, CancellationToken cancellationToken = default);
    Task AddAsync(CommunicationMessage message, CancellationToken cancellationToken = default);
    Task UpdateAsync(CommunicationMessage message, CancellationToken cancellationToken = default);
    Task DeleteAsync(CommunicationMessage message, CancellationToken cancellationToken = default);
}
