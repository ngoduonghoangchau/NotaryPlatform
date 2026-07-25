using NotaryPlatform.Application.Shared.Behaviors;
using NotaryPlatform.Application.Shared.Constants;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Features.Core.Commands.InitiatePasswordReset;

/// <summary>
/// UC-AUTH-05 Step A — a tenant admin triggers a password reset for a user in their tenant. Mints a
/// single-use reset token (BR-AUTH-09), stores only its hash, and emails the reset link. A write ⇒
/// <see cref="ICommand"/> (transactional). Authenticated with permission <c>admin.users.manage</c>.
/// </summary>
public sealed record InitiatePasswordResetCommand(Guid UserId)
    : ICommand, IAuthorizedRequest
{
    public string RequiredPermission => PermissionCodes.Admin.UsersManage;
}
