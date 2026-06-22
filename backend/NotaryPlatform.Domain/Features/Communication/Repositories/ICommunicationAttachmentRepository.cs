using NotaryPlatform.Domain.Features.Communication.Aggregates;

namespace NotaryPlatform.Domain.Features.Communication.Repositories;

public interface ICommunicationAttachmentRepository
{
    Task<CommunicationAttachment?> GetByIdAsync(Guid communicationAttachmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommunicationAttachment>> ListByMessageAsync(Guid communicationMessageId, CancellationToken cancellationToken = default);
    Task AddAsync(CommunicationAttachment attachment, CancellationToken cancellationToken = default);
    Task UpdateAsync(CommunicationAttachment attachment, CancellationToken cancellationToken = default);
    Task DeleteAsync(CommunicationAttachment attachment, CancellationToken cancellationToken = default);
}
