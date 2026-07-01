using Microsoft.AspNetCore.Authorization;
using NotaryPlatform.Infrastructure.Authorization.Claims;
using NotaryPlatform.Infrastructure.Authorization.Requirements;

namespace NotaryPlatform.Infrastructure.Authorization.Handlers;

/// <summary>
/// Grants access when the user's JWT contains the permission code
/// specified by <see cref="PermissionRequirement"/>.
/// Stateless — zero DB calls. Safe to register as Transient.
/// </summary>
internal sealed class PermissionRequirementHandler
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // FindAll returns every "permissions" claim — one per granted code.
        var granted = context.User
            .FindAll(NotaryPlatformClaimTypes.Permissions)
            .Any(c => string.Equals(
                c.Value,
                requirement.PermissionCode,
                StringComparison.OrdinalIgnoreCase));

        if (granted)
            context.Succeed(requirement);

        // Returning CompletedTask without calling Fail() allows other handlers
        // (if any) to still succeed the requirement.
        return Task.CompletedTask;
    }
}
