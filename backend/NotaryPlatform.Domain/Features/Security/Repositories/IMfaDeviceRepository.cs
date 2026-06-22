using NotaryPlatform.Domain.Features.Security.Aggregates;

namespace NotaryPlatform.Domain.Features.Security.Repositories;

public interface IMfaDeviceRepository
{
    Task<MfaDevice?> GetByIdAsync(Guid mfaDeviceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MfaDevice>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(MfaDevice device, CancellationToken cancellationToken = default);
    Task UpdateAsync(MfaDevice device, CancellationToken cancellationToken = default);
    Task DeleteAsync(MfaDevice device, CancellationToken cancellationToken = default);
}
