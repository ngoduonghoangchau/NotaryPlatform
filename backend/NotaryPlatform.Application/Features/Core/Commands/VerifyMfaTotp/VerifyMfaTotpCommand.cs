using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Behaviors;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Features.Core.Commands.VerifyMfaTotp;

/// <summary>
/// UC-AUTH-06 Step B — a signed-in user submits a 6-digit code to activate a pending TOTP device. On
/// success the device becomes verified/primary, any prior verified TOTP is revoked, and single-use
/// recovery codes are issued (once). A write ⇒ <see cref="ICommand{T}"/> (transactional). Authenticated
/// <see cref="IAuthorizedRequest"/> with a null permission (any signed-in user).
/// </summary>
public sealed record VerifyMfaTotpCommand(Guid MfaDeviceId, string Code)
    : ICommand<MfaRecoveryCodesResponse>, IAuthorizedRequest
{
    public string? RequiredPermission => null;   // any authenticated user
}
