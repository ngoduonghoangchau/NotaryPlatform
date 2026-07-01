using Microsoft.AspNetCore.Authorization;

namespace NotaryPlatform.Infrastructure.Authorization.Requirements;

/// <summary>
/// Requires the authenticated user to have at least one Active or Expiring
/// notary commission in the database for their current tenant.
///
/// This requirement performs a DB query — apply only to endpoints that
/// gate notarial act execution, journal entry creation, or seal usage,
/// where stale JWT state is unacceptable.
///
/// Handled by <c>ActiveNotaryRequirementHandler</c>.
/// </summary>
public sealed class ActiveNotaryRequirement : IAuthorizationRequirement
{
    /// <summary>Shared singleton — this requirement has no parameters.</summary>
    internal static readonly ActiveNotaryRequirement Instance = new();
}
