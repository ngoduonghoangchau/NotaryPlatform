using NotaryPlatform.Domain.Features.Communication.Aggregates;

namespace NotaryPlatform.Domain.Features.Communication.Repositories;

public interface ICommunicationDeliveryLogRepository
{
    Task<CommunicationDeliveryLog?> GetByIdAsync(Guid communicationDeliveryLogId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommunicationDeliveryLog>> ListByMessageAsync(Guid communicationMessageId, CancellationToken cancellationToken = default);
    Task AddAsync(CommunicationDeliveryLog deliveryLog, CancellationToken cancellationToken = default);
    Task DeleteAsync(CommunicationDeliveryLog deliveryLog, CancellationToken cancellationToken = default);
}
