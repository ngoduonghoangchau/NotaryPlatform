using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotaryPlatform.Application.Features.Core.Commands.ChangePassword;
using NotaryPlatform.Application.Features.Core.Commands.CompletePasswordReset;
using NotaryPlatform.Application.Features.Core.Commands.EnrollMfaTotp;
using NotaryPlatform.Application.Features.Core.Commands.InitiatePasswordReset;
using NotaryPlatform.Application.Features.Core.Commands.Login;
using NotaryPlatform.Application.Features.Core.Commands.Logout;
using NotaryPlatform.Application.Features.Core.Commands.RefreshToken;
using NotaryPlatform.Application.Features.Core.Commands.RemoveTrustedDevice;
using NotaryPlatform.Application.Features.Core.Commands.VerifyLoginMfa;
using NotaryPlatform.Application.Features.Core.Commands.VerifyMfaTotp;
using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Features.Core.Queries.GetTrustedDevices;
using NotaryPlatform.Application.Shared.Models.Responses;

namespace NotaryPlatform.API.Controllers.v1;

/// <summary>
/// Authentication endpoints (UC-AUTH-01…). Thin HTTP adapter — all logic lives behind MediatR.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender) => _sender = sender;

    /// <summary>
    /// UC-AUTH-01 / UC-AUTH-07 — authenticate with email + password. A user without MFA receives access +
    /// refresh tokens (<c>status = authenticated</c>); a user with MFA enabled receives a short-lived
    /// challenge instead (<c>status = mfa_required</c>) and must complete <c>POST /auth/login/mfa</c>.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status423Locked)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new LoginCommand(request.TenantCode, request.Email, request.Password, request.DeviceName, request.Fingerprint),
            cancellationToken);

        var message = result.Status == LoginResponse.StatusMfaRequired
            ? "Multi-factor authentication required."
            : "Login successful.";

        return Ok(ApiResponse<LoginResponse>.Ok(result, message));
    }

    /// <summary>
    /// UC-AUTH-07 — complete an MFA login: submit the <c>mfaToken</c> from the login challenge plus a
    /// 6-digit TOTP code or a recovery code, and receive access + refresh tokens. Anonymous (the challenge
    /// token is the presented credential).
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login/mfa")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status423Locked)]
    public async Task<IActionResult> VerifyLoginMfa(
        [FromBody] VerifyLoginMfaRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new VerifyLoginMfaCommand(
                request.MfaToken,
                request.Code,
                request.Fingerprint,
                request.TrustDevice,
                request.DeviceName,
                request.Platform,
                request.Browser),
            cancellationToken);

        return Ok(ApiResponse<LoginResponse>.Ok(result, "Login successful."));
    }

    /// <summary>
    /// UC-AUTH-02 — exchange a valid refresh token for a new access token and a rotated refresh token.
    /// Anonymous: the caller's access token has expired, so the refresh token is the presented credential.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RefreshTokenCommand(request.RefreshToken), cancellationToken);

        return Ok(ApiResponse<RefreshTokenResponse>.Ok(result, "Token refreshed."));
    }

    /// <summary>
    /// UC-AUTH-03 — sign out: revoke the current session's refresh token (or all sessions with
    /// <c>allDevices</c>) and invalidate the user's auth cache. Requires a valid access token; an
    /// unauthenticated call is rejected with 401 by the authorization pipeline.
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new LogoutCommand(request.RefreshToken, request.AllDevices), cancellationToken);

        return Ok(ApiResponse.Ok("Logged out."));
    }

    /// <summary>
    /// UC-AUTH-04 — change your own password: re-verify the current password, enforce the complexity
    /// policy on the new one, store it, and sign out every session (they must re-authenticate).
    /// Requires a valid access token.
    /// </summary>
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new ChangePasswordCommand(request.CurrentPassword, request.NewPassword), cancellationToken);

        return Ok(ApiResponse.Ok("Password changed."));
    }

    /// <summary>
    /// UC-AUTH-05 Step A — an admin (permission <c>admin.users.manage</c>) initiates a password reset
    /// for a user in their tenant: emails a single-use, 1-hour reset link. Requires a valid access token.
    /// </summary>
    [HttpPost("password-reset/initiate")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> InitiatePasswordReset(
        [FromBody] InitiatePasswordResetRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new InitiatePasswordResetCommand(request.UserId), cancellationToken);

        return Ok(ApiResponse.Ok("Password reset link sent."));
    }

    /// <summary>
    /// UC-AUTH-05 Step B — the user completes a reset with the emailed token and a new password.
    /// Anonymous (the token is the credential). On success every session is revoked (BR-AUTH-06).
    /// </summary>
    [AllowAnonymous]
    [HttpPost("password-reset/complete")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompletePasswordReset(
        [FromBody] CompletePasswordResetRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new CompletePasswordResetCommand(request.Token, request.NewPassword), cancellationToken);

        return Ok(ApiResponse.Ok("Password has been reset."));
    }

    /// <summary>
    /// UC-AUTH-06 Step A — a signed-in user begins TOTP MFA enrollment. Returns the raw secret and an
    /// <c>otpauth://</c> URI once (for the QR); the device stays pending until <c>verify</c> proves a code.
    /// Requires a valid access token.
    /// </summary>
    [HttpPost("mfa/totp/enroll")]
    [ProducesResponseType(typeof(ApiResponse<MfaEnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> EnrollMfaTotp(
        [FromBody] EnrollMfaRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new EnrollMfaTotpCommand(request.Label), cancellationToken);

        return Ok(ApiResponse<MfaEnrollmentResponse>.Ok(result, "Scan the secret, then verify a code to finish."));
    }

    /// <summary>
    /// UC-AUTH-06 Step B — activate a pending TOTP device with a 6-digit code. On success the device
    /// becomes primary, any prior verified TOTP is revoked, and single-use recovery codes are returned
    /// once. Requires a valid access token.
    /// </summary>
    [HttpPost("mfa/totp/verify")]
    [ProducesResponseType(typeof(ApiResponse<MfaRecoveryCodesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status423Locked)]
    public async Task<IActionResult> VerifyMfaTotp(
        [FromBody] VerifyMfaRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new VerifyMfaTotpCommand(request.MfaDeviceId, request.Code), cancellationToken);

        return Ok(ApiResponse<MfaRecoveryCodesResponse>.Ok(result, "MFA enabled. Store these recovery codes safely — shown once."));
    }

    /// <summary>
    /// UC-AUTH-08 — list the signed-in user's own trusted devices (the ones that can bypass the MFA
    /// challenge, BR-AUTH-08). Requires a valid access token; only the caller's own devices are returned.
    /// </summary>
    [HttpGet("trusted-devices")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TrustedDeviceResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTrustedDevices(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTrustedDevicesQuery(), cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<TrustedDeviceResponse>>.Ok(result, "Trusted devices retrieved."));
    }

    /// <summary>
    /// UC-AUTH-08 — remove (soft-revoke) one of the signed-in user's own trusted devices, so it can no
    /// longer bypass MFA. Requires a valid access token. Returns 404 when the device is not the caller's
    /// (ownership guard / anti-enumeration).
    /// </summary>
    [HttpDelete("trusted-devices/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTrustedDevice(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new RemoveTrustedDeviceCommand(id), cancellationToken);

        return Ok(ApiResponse.Ok("Trusted device removed."));
    }
}

/// <summary>
/// Request body for <c>POST /api/v1/auth/login</c>. The trusted-device fields are optional (UC-AUTH-08):
/// <c>Fingerprint</c> drives the MFA bypass (BR-AUTH-08); <c>Platform</c>/<c>Browser</c> are accepted for
/// client symmetry with the verify request but are persisted only when a device is trusted (at
/// <c>login/mfa</c>). Older clients that omit all three are unaffected (additive change).
/// </summary>
public sealed record LoginRequest(
    string TenantCode,
    string Email,
    string Password,
    string? DeviceName,
    string? Fingerprint = null,
    string? Platform = null,
    string? Browser = null);

/// <summary>Request body for <c>POST /api/v1/auth/refresh</c>.</summary>
public sealed record RefreshRequest(string RefreshToken);

/// <summary>Request body for <c>POST /api/v1/auth/logout</c>.</summary>
public sealed record LogoutRequest(string? RefreshToken, bool AllDevices = false);

/// <summary>Request body for <c>POST /api/v1/auth/change-password</c>.</summary>
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

/// <summary>Request body for <c>POST /api/v1/auth/password-reset/initiate</c>.</summary>
public sealed record InitiatePasswordResetRequest(Guid UserId);

/// <summary>Request body for <c>POST /api/v1/auth/password-reset/complete</c>.</summary>
public sealed record CompletePasswordResetRequest(string Token, string NewPassword);

/// <summary>Request body for <c>POST /api/v1/auth/mfa/totp/enroll</c> (label optional).</summary>
public sealed record EnrollMfaRequest(string? Label);

/// <summary>Request body for <c>POST /api/v1/auth/mfa/totp/verify</c>.</summary>
public sealed record VerifyMfaRequest(Guid MfaDeviceId, string Code);

/// <summary>
/// Request body for <c>POST /api/v1/auth/login/mfa</c> (UC-AUTH-07 + UC-AUTH-08). The trusted-device fields
/// are optional: set <c>TrustDevice = true</c> with a valid <c>Fingerprint</c> to register the current
/// device as trusted after this MFA verification, so later logins skip the challenge (BR-AUTH-08).
/// <c>DeviceName</c>/<c>Platform</c>/<c>Browser</c> are stored as device hints.
/// </summary>
public sealed record VerifyLoginMfaRequest(
    string MfaToken,
    string Code,
    string? Fingerprint = null,
    bool TrustDevice = false,
    string? DeviceName = null,
    string? Platform = null,
    string? Browser = null);
