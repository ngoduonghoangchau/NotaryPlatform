using NotaryPlatform.Domain.Features.Security.Aggregates;

namespace NotaryPlatform.Domain.Features.Security.Repositories;

public interface ISealRevocationRepository
{
    Task<SealRevocation?> GetByIdAsync(Guid sealRevocationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SealRevocation>> ListBySealAsync(Guid sealId, CancellationToken cancellationToken = default);
    Task AddAsync(SealRevocation revocation, CancellationToken cancellationToken = default);
    Task DeleteAsync(SealRevocation revocation, CancellationToken cancellationToken = default);
}
