using NotaryPlatform.Domain.Features.Core.Aggregates;

namespace NotaryPlatform.Domain.Features.Core.Repositories;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid permissionId, CancellationToken cancellationToken = default);
    Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> ListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default);
    Task DeleteAsync(Permission permission, CancellationToken cancellationToken = default);
}
