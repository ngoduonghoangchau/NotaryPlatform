using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Features.Core.Commands.Login;

/// <summary>
/// UC-AUTH-01 — authenticates a user with email + password and issues access + refresh tokens.
///
/// A write (rotates/creates a refresh token, stamps last-login), so it is an <see cref="ICommand{T}"/>
/// and runs inside a DB transaction. It is intentionally NOT <c>IAuthorizedRequest</c> — login is the
/// anonymous entry point that mints the token every later request is authorized by.
///
/// <paramref name="TenantCode"/> disambiguates the per-tenant-unique login email (Decision D1).
/// <paramref name="DeviceName"/> is optional; when present it drives one-token-per-device (BR-AUTH-07).
/// </summary>
public sealed record LoginCommand(
    string TenantCode,
    string Email,
    string Password,
    string? DeviceName) : ICommand<LoginResponse>;
