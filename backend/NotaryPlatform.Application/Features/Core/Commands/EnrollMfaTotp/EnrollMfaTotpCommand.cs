using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Behaviors;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Features.Core.Commands.EnrollMfaTotp;

/// <summary>
/// UC-AUTH-06 Step A — a signed-in user begins TOTP MFA enrollment. Generates a secret, stores it
/// encrypted, and returns the raw secret + <c>otpauth://</c> URI once (for the QR). A write ⇒
/// <see cref="ICommand{T}"/> (transactional). Authenticated <see cref="IAuthorizedRequest"/> with a null
/// permission (any signed-in user) — the access token proves identity.
/// </summary>
public sealed record EnrollMfaTotpCommand(string? Label)
    : ICommand<MfaEnrollmentResponse>, IAuthorizedRequest
{
    public string? RequiredPermission => null;   // any authenticated user
}
