using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Infrastructure.Authorization.Evaluators;

/// <summary>
/// Imperative permission checker for use inside CQRS handlers and domain services.
/// Wraps <see cref="ICurrentUser"/> to keep multi-permission logic in one place
/// and avoid scattering raw <c>HasPermission</c> calls across the Application layer.
///
/// Prefer <see cref="ICurrentUser.RequirePermission"/> for single-permission guards
/// that should throw on failure. Use this evaluator when you need boolean checks,
/// multi-permission logic, or aggregate queries before deciding a course of action.
/// </summary>
public sealed class PermissionEvaluator
{
    private readonly ICurrentUser _currentUser;

    public PermissionEvaluator(ICurrentUser currentUser) => _currentUser = currentUser;

    /// <summary>Returns <c>true</c> if the user holds the specified permission.</summary>
    public bool Has(string permissionCode) => _currentUser.HasPermission(permissionCode);

    /// <summary>
    /// Returns <c>true</c> if the user holds <b>every</b> permission in <paramref name="permissionCodes"/>.
    /// </summary>
    public bool HasAll(params string[] permissionCodes) =>
        permissionCodes.All(_currentUser.HasPermission);

    /// <summary>
    /// Returns <c>true</c> if the user holds <b>at least one</b> permission from <paramref name="permissionCodes"/>.
    /// </summary>
    public bool HasAny(params string[] permissionCodes) =>
        permissionCodes.Any(_currentUser.HasPermission);

    /// <summary>
    /// Returns all permission codes currently granted to the user via their JWT.
    /// Useful for building capability-aware UI responses.
    /// </summary>
    public IReadOnlyList<string> GetGrantedPermissions() => _currentUser.Permissions;

    /// <summary>
    /// Returns <c>true</c> if the user is acting within the specified tenant.
    /// A user can only access data belonging to their own tenant.
    /// </summary>
    public bool CanAccessTenant(Guid resourceTenantId) =>
        _currentUser.TenantId.HasValue
        && _currentUser.TenantId.Value == resourceTenantId;

    /// <summary>
    /// Returns the current user's tenant ID, or <c>null</c> if the claim is absent.
    /// </summary>
    public Guid? CurrentTenantId => _currentUser.TenantId;

    /// <summary>
    /// Returns the current user's ID, or <c>null</c> if the claim is absent.
    /// </summary>
    public Guid? CurrentUserId => _currentUser.UserId;
}
