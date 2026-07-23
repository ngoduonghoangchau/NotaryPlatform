using NotaryPlatform.Application.Shared.Behaviors;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Features.Core.Commands.ChangePassword;

/// <summary>
/// UC-AUTH-04 — a signed-in user changes their own password. It re-verifies the current password,
/// enforces BR-AUTH-01 complexity on the new one, stores the new hash, and revokes all refresh tokens
/// (BR-AUTH-06). A write ⇒ <see cref="ICommand"/> (transactional). Authenticated
/// <see cref="IAuthorizedRequest"/> with a null permission (any signed-in user): the access token
/// proves identity and the current password re-confirms it.
/// </summary>
public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword)
    : ICommand, IAuthorizedRequest
{
    public string? RequiredPermission => null;   // any authenticated user
}
