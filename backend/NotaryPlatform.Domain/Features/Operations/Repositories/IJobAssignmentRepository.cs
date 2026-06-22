using NotaryPlatform.Domain.Features.Operations.Aggregates;

namespace NotaryPlatform.Domain.Features.Operations.Repositories;

public interface IJobAssignmentRepository
{
    Task<JobAssignment?> GetByIdAsync(Guid jobAssignmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobAssignment>> ListByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobAssignment>> ListByNotaryIdAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task AddAsync(JobAssignment assignment, CancellationToken cancellationToken = default);
    Task UpdateAsync(JobAssignment assignment, CancellationToken cancellationToken = default);
    Task DeleteAsync(JobAssignment assignment, CancellationToken cancellationToken = default);
}
