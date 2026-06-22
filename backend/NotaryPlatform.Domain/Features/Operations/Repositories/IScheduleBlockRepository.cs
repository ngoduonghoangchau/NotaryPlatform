using NotaryPlatform.Domain.Features.Operations.Aggregates;

namespace NotaryPlatform.Domain.Features.Operations.Repositories;

public interface IScheduleBlockRepository
{
    Task<ScheduleBlock?> GetByIdAsync(Guid scheduleBlockId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleBlock>> ListByNotaryAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleBlock>> ListByJobAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task AddAsync(ScheduleBlock scheduleBlock, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScheduleBlock scheduleBlock, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScheduleBlock scheduleBlock, CancellationToken cancellationToken = default);
}
