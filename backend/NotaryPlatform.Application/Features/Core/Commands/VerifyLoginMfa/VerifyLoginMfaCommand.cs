using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Features.Core.Commands.VerifyLoginMfa;

/// <summary>
/// UC-AUTH-07 Step B — completes an MFA login. The caller presents the <paramref name="MfaToken"/> from the
/// login challenge plus a <paramref name="Code"/> (a 6-digit TOTP <b>or</b> an <c>xxxx-xxxx</c> recovery
/// code). On success the server issues the access + refresh tokens. Anonymous (the challenge token is the
/// presented credential). A write (issues a refresh token, consumes a recovery code) ⇒ transactional
/// <see cref="ICommand{T}"/> — intentionally NOT <c>IAuthorizedRequest</c>.
///
/// UC-AUTH-08 — when <paramref name="TrustDevice"/> is true and a valid <paramref name="Fingerprint"/> is
/// supplied, the current device is registered as trusted (after this proven MFA) so later logins skip the
/// challenge (BR-AUTH-08). The optional <paramref name="DeviceName"/>/<paramref name="Platform"/>/
/// <paramref name="Browser"/> are stored as device hints.
/// </summary>
public sealed record VerifyLoginMfaCommand(
    string MfaToken,
    string Code,
    string? Fingerprint = null,
    bool TrustDevice = false,
    string? DeviceName = null,
    string? Platform = null,
    string? Browser = null) : ICommand<LoginResponse>;
