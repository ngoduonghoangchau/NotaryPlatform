using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.ValueObjects;

namespace NotaryPlatform.Domain.Features.Security.Repositories;

public interface ITrustedDeviceRepository
{
    Task<TrustedDevice?> GetByIdAsync(Guid trustedDeviceId, CancellationToken cancellationToken = default);
    Task<TrustedDevice?> GetByFingerprintAsync(DeviceFingerprint fingerprint, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrustedDevice>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(TrustedDevice device, CancellationToken cancellationToken = default);
    Task UpdateAsync(TrustedDevice device, CancellationToken cancellationToken = default);
    Task DeleteAsync(TrustedDevice device, CancellationToken cancellationToken = default);
}
