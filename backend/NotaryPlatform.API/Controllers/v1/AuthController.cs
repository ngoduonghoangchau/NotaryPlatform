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
using NotaryPlatform.Application.Features.Core.Commands.VerifyMfaTotp;
using NotaryPlatform.Application.Features.Core.DTOs;
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
    /// UC-AUTH-01 — authenticate with email + password and receive access + refresh tokens.
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
            new LoginCommand(request.TenantCode, request.Email, request.Password, request.DeviceName),
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
}

/// <summary>Request body for <c>POST /api/v1/auth/login</c>.</summary>
public sealed record LoginRequest(string TenantCode, string Email, string Password, string? DeviceName);

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
