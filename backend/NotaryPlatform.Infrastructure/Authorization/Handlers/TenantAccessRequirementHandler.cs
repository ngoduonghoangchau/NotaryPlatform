using Microsoft.AspNetCore.Authorization;
using NotaryPlatform.Infrastructure.Authorization.Claims;
using NotaryPlatform.Infrastructure.Authorization.Requirements;

namespace NotaryPlatform.Infrastructure.Authorization.Handlers;

/// <summary>
/// Grants access when the user's JWT contains a valid, non-empty tenant ID claim (<c>tid</c>).
/// Stateless — zero DB calls. Safe to register as Transient.
/// </summary>
internal sealed class TenantAccessRequirementHandler
    : AuthorizationHandler<TenantAccessRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantAccessRequirement requirement)
    {
        var tidValue = context.User.FindFirst(NotaryPlatformClaimTypes.TenantId)?.Value;

        if (Guid.TryParse(tidValue, out var tenantId) && tenantId != Guid.Empty)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
