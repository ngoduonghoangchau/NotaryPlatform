namespace NotaryPlatform.Infrastructure.Authorization.Policies;

/// <summary>
/// Named authorization policy constants.
///
/// Usage in controllers:
/// <code>
/// [Authorize(Policy = AuthorizationPolicies.TenantMember)]
/// [Authorize(Policy = AuthorizationPolicies.Permission(PermissionCodes.Crm.CustomersRead))]
/// </code>
///
/// Usage in handlers via IAuthorizationService:
/// <code>
/// await _authService.AuthorizeAsync(user, AuthorizationPolicies.ActiveNotary);
/// </code>
/// </summary>
public static class AuthorizationPolicies
{
    // ── Named policies ────────────────────────────────────────────────────────────

    /// <summary>
    /// Requires a valid authenticated JWT.
    /// The minimum gate for any endpoint — does not check tenant or permissions.
    /// </summary>
    public const string Authenticated = "Authenticated";

    /// <summary>
    /// Requires the user to carry a non-empty tenant ID claim (<c>tid</c>).
    /// Applied to all tenant-scoped endpoints.
    /// </summary>
    public const string TenantMember = "TenantMember";

    /// <summary>
    /// Requires the user to hold an <c>Active</c> or <c>Expiring</c> notary commission
    /// in the database for their current tenant.
    /// Applied to endpoints that perform or gate notarial acts, journal entries,
    /// and seal usage — contexts where stale JWT claim data is legally unacceptable.
    /// </summary>
    public const string ActiveNotary = "ActiveNotary";

    // ── Permission policy name builder ────────────────────────────────────────────

    /// <summary>
    /// Derives a policy name for a single permission code.
    /// A policy is registered for every constant in <see cref="PermissionMaps.PermissionCodes"/>
    /// via <see cref="PolicyRegistrationExtensions.AddNotaryPlatformAuthorization"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// [Authorize(Policy = AuthorizationPolicies.Permission(PermissionCodes.Crm.CustomersRead))]
    /// </code>
    /// </example>
    public static string Permission(string permissionCode) => $"Permission:{permissionCode}";
}
