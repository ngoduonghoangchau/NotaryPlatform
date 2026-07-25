using NotaryPlatform.Application.Features.Core.DTOs;

namespace NotaryPlatform.Application.Features.Core.Services;

/// <summary>
/// Issues an authenticated session (access token + persisted refresh token) for a subject whose identity
/// has already been proven — by password (UC-AUTH-01, no-MFA path) or by password + MFA code (UC-AUTH-07).
/// Extracted so both paths mint sessions <b>identically</b> (same claims, same BR-AUTH-07 device revoke,
/// same last-login stamp). Writes are tracked and committed by <c>TransactionBehavior</c>.
/// </summary>
public interface IAuthSessionIssuer
{
    Task<AuthSession> IssueAsync(SessionSubject subject, string? deviceName, CancellationToken cancellationToken = default);
}

/// <summary>The already-authenticated user a session is being issued for.</summary>
public sealed record SessionSubject(
    Guid UserId,
    Guid TenantId,
    Guid? BranchId,
    string Email,
    string DisplayName);
