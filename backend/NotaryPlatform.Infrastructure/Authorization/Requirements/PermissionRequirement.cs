using Microsoft.AspNetCore.Authorization;

namespace NotaryPlatform.Infrastructure.Authorization.Requirements;

/// <summary>
/// Requires the authenticated user to hold a specific permission code
/// embedded as a <c>permissions</c> claim in the JWT.
/// Handled by <c>PermissionRequirementHandler</c> (stateless — no DB call).
/// </summary>
public sealed record PermissionRequirement(string PermissionCode) : IAuthorizationRequirement;
