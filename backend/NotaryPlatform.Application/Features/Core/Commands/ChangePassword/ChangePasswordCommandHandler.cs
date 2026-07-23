using MediatR;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Shared.Exceptions;

namespace NotaryPlatform.Application.Features.Core.Commands.ChangePassword;

/// <summary>
/// Executes UC-AUTH-04. See <c>docs/usecase/Auth/UC-AUTH-04/implement_plan.md</c>.
///
/// Re-verifies the current password (an attacker holding only a stolen access token still cannot change
/// it), stores the new BCrypt hash, and revokes ALL refresh tokens (BR-AUTH-06) so every session must
/// re-authenticate. The hash update + session revoke are atomic (one transaction). The stateless access
/// token stays valid until its ≤ 60-minute expiry — the guarantee is that no session can refresh after.
/// </summary>
internal sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private const string CurrentPasswordField = "currentPassword";

    private readonly IAuthRepository _auth;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUser _currentUser;

    public ChangePasswordCommandHandler(
        IAuthRepository auth,
        IPasswordHasher passwordHasher,
        ICurrentUser currentUser)
    {
        _auth = auth;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"ChangePasswordCommandHandler: Handling change password for user {_currentUser.UserId} in tenant {_currentUser.TenantId}");
        // AuthorizationBehavior guaranteed IsAuthenticated, but a validly-signed token can still lack the
        // uid/tid claims — treat a missing required claim as unauthenticated (401) instead of crashing (500).
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var tenantId = _currentUser.TenantId ?? throw new UnauthorizedException();

        // Load the current hash; a user that vanished since the token was issued ⇒ 401.
        var currentHash = await _auth.FindPasswordHashAsync(userId, tenantId, cancellationToken)
            ?? throw new UnauthorizedException();

        // Re-verify the current password. Wrong ⇒ 400 field error (not 401 — the request IS authenticated).
        if (!_passwordHasher.Verify(request.CurrentPassword, currentHash))
            throw new ValidationException(CurrentPasswordField, "Current password is incorrect.", "INCORRECT_PASSWORD");

        // Store the new hash and revoke every session (BR-AUTH-06). Tracked writes — TransactionBehavior
        // commits them atomically after this returns.
        var newHash = _passwordHasher.Hash(request.NewPassword);
        await _auth.UpdatePasswordHashAsync(userId, newHash, cancellationToken);
        await _auth.RevokeRefreshTokensAsync(userId, tokenHash: null, allDevices: true, cancellationToken);
    }
}
