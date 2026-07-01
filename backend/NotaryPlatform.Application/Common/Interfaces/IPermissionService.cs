namespace NotaryPlatform.Application.Common.Interfaces;

/// <summary>
/// Resolves a user's roles and permission codes from the data store.
/// Called during login / token-refresh to embed claims in the JWT.
/// Defined in Application so use cases can call it without knowing
/// whether the implementation queries PostgreSQL, a cache, or any other store.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Returns all active permission codes granted to the user
    /// through any of their roles within the specified tenant.
    /// </summary>
    Task<IReadOnlyList<string>> GetPermissionsAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the role codes held by the user within the specified tenant.
    /// </summary>
    Task<IReadOnlyList<string>> GetRolesAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
