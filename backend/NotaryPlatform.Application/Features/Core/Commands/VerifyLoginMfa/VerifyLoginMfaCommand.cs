using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Features.Core.Commands.VerifyLoginMfa;

/// <summary>
/// UC-AUTH-07 Step B — completes an MFA login. The caller presents the <paramref name="MfaToken"/> from the
/// login challenge plus a <paramref name="Code"/> (a 6-digit TOTP <b>or</b> an <c>xxxx-xxxx</c> recovery
/// code). On success the server issues the access + refresh tokens. Anonymous (the challenge token is the
/// presented credential). A write (issues a refresh token, consumes a recovery code) ⇒ transactional
/// <see cref="ICommand{T}"/> — intentionally NOT <c>IAuthorizedRequest</c>.
/// </summary>
public sealed record VerifyLoginMfaCommand(string MfaToken, string Code) : ICommand<LoginResponse>;
