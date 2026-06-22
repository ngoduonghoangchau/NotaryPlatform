using NotaryPlatform.Domain.Features.Communication.Aggregates;

namespace NotaryPlatform.Domain.Features.Communication.Repositories;

public interface ICommunicationParticipantRepository
{
    Task<CommunicationParticipant?> GetByIdAsync(Guid communicationParticipantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommunicationParticipant>> ListByThreadAsync(Guid communicationThreadId, CancellationToken cancellationToken = default);
    Task AddAsync(CommunicationParticipant participant, CancellationToken cancellationToken = default);
    Task UpdateAsync(CommunicationParticipant participant, CancellationToken cancellationToken = default);
    Task DeleteAsync(CommunicationParticipant participant, CancellationToken cancellationToken = default);
}
