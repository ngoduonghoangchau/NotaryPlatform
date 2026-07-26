using NotaryPlatform.Application.Shared.Behaviors;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Features.Core.Commands.RemoveTrustedDevice;

/// <summary>
/// UC-AUTH-08 — the signed-in user removes (soft-revokes) one of their own trusted devices, so it can no
/// longer bypass MFA. A write ⇒ transactional <see cref="ICommand"/>. Authenticated
/// <see cref="IAuthorizedRequest"/> with a null permission (any signed-in user); the handler scopes the
/// revoke to <c>ICurrentUser</c> — a caller can never revoke another user's device.
/// </summary>
public sealed record RemoveTrustedDeviceCommand(Guid TrustedDeviceId) : ICommand, IAuthorizedRequest
{
    public string? RequiredPermission => null;   // any authenticated user
}
