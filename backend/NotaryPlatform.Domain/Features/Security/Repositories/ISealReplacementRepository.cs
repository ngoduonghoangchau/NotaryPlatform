using NotaryPlatform.Domain.Features.Security.Aggregates;

namespace NotaryPlatform.Domain.Features.Security.Repositories;

public interface ISealReplacementRepository
{
    Task<SealReplacement?> GetByIdAsync(Guid sealReplacementId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SealReplacement>> ListByOldSealAsync(Guid oldSealId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SealReplacement>> ListByNewSealAsync(Guid newSealId, CancellationToken cancellationToken = default);
    Task AddAsync(SealReplacement replacement, CancellationToken cancellationToken = default);
    Task DeleteAsync(SealReplacement replacement, CancellationToken cancellationToken = default);
}
