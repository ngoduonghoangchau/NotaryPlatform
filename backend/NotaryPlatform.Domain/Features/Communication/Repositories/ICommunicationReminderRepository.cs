using NotaryPlatform.Domain.Features.Communication.Aggregates;

namespace NotaryPlatform.Domain.Features.Communication.Repositories;

public interface ICommunicationReminderRepository
{
    Task<CommunicationReminder?> GetByIdAsync(Guid communicationReminderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommunicationReminder>> ListByThreadAsync(Guid communicationThreadId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommunicationReminder>> ListByStatusAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(CommunicationReminder reminder, CancellationToken cancellationToken = default);
    Task UpdateAsync(CommunicationReminder reminder, CancellationToken cancellationToken = default);
    Task DeleteAsync(CommunicationReminder reminder, CancellationToken cancellationToken = default);
}
