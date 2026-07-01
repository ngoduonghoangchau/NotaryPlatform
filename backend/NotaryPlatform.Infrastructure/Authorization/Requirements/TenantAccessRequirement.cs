using Microsoft.AspNetCore.Authorization;

namespace NotaryPlatform.Infrastructure.Authorization.Requirements;

/// <summary>
/// Requires the authenticated user to carry a valid, non-empty tenant ID claim (<c>tid</c>).
/// Prevents tokens issued without a tenant context (e.g., system-bootstrap tokens)
/// from accessing tenant-scoped endpoints.
/// Handled by <c>TenantAccessRequirementHandler</c> (stateless — no DB call).
/// </summary>
public sealed class TenantAccessRequirement : IAuthorizationRequirement
{
    /// <summary>Shared singleton — this requirement has no parameters.</summary>
    internal static readonly TenantAccessRequirement Instance = new();
}
