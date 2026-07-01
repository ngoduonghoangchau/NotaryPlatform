using NotaryPlatform.Application.Common.Interfaces;

namespace NotaryPlatform.Infrastructure.Authorization.Evaluators;

/// <summary>
/// Evaluates whether the current user may access a specific resource by combining
/// tenant isolation and permission checks.
/// Inject this into CQRS handlers that must enforce resource-level access control
/// beyond what the global authorization policy can express.
///
/// Tenant isolation is always enforced first — no permission grants cross-tenant access.
/// </summary>
public sealed class ResourceAccessEvaluator
{
    private readonly ICurrentUser _currentUser;
    private readonly PermissionEvaluator _permissions;

    public ResourceAccessEvaluator(ICurrentUser currentUser, PermissionEvaluator permissions)
    {
        _currentUser = currentUser;
        _permissions = permissions;
    }

    /// <summary>
    /// Returns <c>true</c> if the user may <b>read</b> a resource owned by <paramref name="resourceTenantId"/>:
    /// the user belongs to the same tenant AND holds <paramref name="readPermission"/>.
    /// </summary>
    public bool CanRead(Guid resourceTenantId, string readPermission) =>
        _permissions.CanAccessTenant(resourceTenantId)
        && _currentUser.HasPermission(readPermission);

    /// <summary>
    /// Returns <c>true</c> if the user may <b>write</b> (create/update) a resource
    /// owned by <paramref name="resourceTenantId"/> and holds <paramref name="writePermission"/>.
    /// </summary>
    public bool CanWrite(Guid resourceTenantId, string writePermission) =>
        _permissions.CanAccessTenant(resourceTenantId)
        && _currentUser.HasPermission(writePermission);

    /// <summary>
    /// Returns <c>true</c> if the user is the <b>owner</b> of the resource
    /// (matched by <paramref name="resourceOwnerId"/> == current user ID)
    /// AND holds the specified <paramref name="permissionCode"/>.
    ///
    /// Typical use: a Notary may edit their own notarial act but not another Notary's.
    /// </summary>
    public bool IsOwnerWithPermission(Guid resourceOwnerId, string permissionCode) =>
        _currentUser.UserId.HasValue
        && _currentUser.UserId.Value == resourceOwnerId
        && _currentUser.HasPermission(permissionCode);

    /// <summary>
    /// Returns <c>true</c> if the user either <b>owns</b> the resource
    /// OR holds an elevated <paramref name="elevatedPermission"/> that allows access
    /// to any resource within the tenant.
    ///
    /// Typical use: a Notary can read their own acts; an admin can read all acts.
    /// </summary>
    public bool IsOwnerOrHasElevatedPermission(Guid resourceOwnerId, string elevatedPermission) =>
        (_currentUser.UserId.HasValue && _currentUser.UserId.Value == resourceOwnerId)
        || _currentUser.HasPermission(elevatedPermission);

    /// <summary>
    /// Returns <c>true</c> if the user may access the resource considering both
    /// tenant isolation and ownership-or-elevated-permission logic.
    /// This is the most common combined check for shared resources.
    /// </summary>
    public bool CanAccessResource(
        Guid resourceTenantId,
        Guid resourceOwnerId,
        string elevatedPermission) =>
        _permissions.CanAccessTenant(resourceTenantId)
        && IsOwnerOrHasElevatedPermission(resourceOwnerId, elevatedPermission);
}
