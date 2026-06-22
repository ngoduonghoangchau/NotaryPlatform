using NotaryPlatform.Domain.Features.Communication.Aggregates;

namespace NotaryPlatform.Domain.Features.Communication.Repositories;

public interface ICallLogRepository
{
    Task<CallLog?> GetByIdAsync(Guid callLogId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CallLog>> ListByThreadAsync(Guid communicationThreadId, CancellationToken cancellationToken = default);
    Task AddAsync(CallLog callLog, CancellationToken cancellationToken = default);
    Task UpdateAsync(CallLog callLog, CancellationToken cancellationToken = default);
    Task DeleteAsync(CallLog callLog, CancellationToken cancellationToken = default);
}
