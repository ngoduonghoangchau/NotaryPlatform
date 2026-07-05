using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Features.Core.Commands.RefreshToken;

/// <summary>
/// UC-AUTH-02 — exchanges a valid refresh token for a new access token and a rotated refresh token.
///
/// It is a write (it rotates the refresh token, BR-AUTH-04), so it is an <see cref="ICommand{T}"/> and
/// runs inside a DB transaction. Like login it is intentionally NOT <c>IAuthorizedRequest</c>: the
/// caller's access token has expired, so the refresh token itself is the presented credential.
/// </summary>
public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponse>;
