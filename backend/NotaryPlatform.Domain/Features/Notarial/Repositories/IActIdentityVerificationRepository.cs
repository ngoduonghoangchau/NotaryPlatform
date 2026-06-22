using NotaryPlatform.Domain.Features.Notarial.Aggregates;

namespace NotaryPlatform.Domain.Features.Notarial.Repositories;

public interface IActIdentityVerificationRepository
{
    Task<ActIdentityVerification?> GetByIdAsync(Guid actIdentityVerificationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ActIdentityVerification>> ListByActIdAsync(Guid notarialActId, CancellationToken cancellationToken = default);
    Task AddAsync(ActIdentityVerification verification, CancellationToken cancellationToken = default);
    Task UpdateAsync(ActIdentityVerification verification, CancellationToken cancellationToken = default);
    Task DeleteAsync(ActIdentityVerification verification, CancellationToken cancellationToken = default);
}
