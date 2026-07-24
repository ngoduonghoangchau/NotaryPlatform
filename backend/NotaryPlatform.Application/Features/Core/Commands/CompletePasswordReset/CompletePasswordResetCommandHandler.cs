using MediatR;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Models.Responses;

namespace NotaryPlatform.Application.Features.Core.Commands.CompletePasswordReset;

/// <summary>
/// Executes UC-AUTH-05 Step B. See <c>docs/usecase/Auth/UC-AUTH-05/implement_plan.md</c>.
///
/// Validates the reset token (exists + unused + unexpired — BR-AUTH-09; one generic failure to prevent
/// enumeration), stores the new BCrypt hash, marks the token used (single-use), and revokes ALL of the
/// user's refresh tokens (BR-AUTH-06). All writes are atomic (one transaction).
/// </summary>
internal sealed class CompletePasswordResetCommandHandler : IRequestHandler<CompletePasswordResetCommand>
{
    private const string TokenField = "token";

    private readonly IAuthRepository _auth;
    private readonly IResetTokenService _tokens;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTime _clock;

    public CompletePasswordResetCommandHandler(
        IAuthRepository auth,
        IResetTokenService tokens,
        IPasswordHasher passwordHasher,
        IDateTime clock)
    {
        _auth = auth;
        _tokens = tokens;
        _passwordHasher = passwordHasher;
        _clock = clock;
    }

    public async Task Handle(CompletePasswordResetCommand request, CancellationToken cancellationToken)
    {
        var hash = _tokens.HashResetToken(request.Token);
        var token = await _auth.FindPasswordResetTokenByHashAsync(hash, cancellationToken);

        // BR-AUTH-09: must exist, be unused, and be unexpired — one generic failure (anti-enumeration).
        if (token is null || token.UsedAtUtc is not null || token.ExpiresAtUtc <= _clock.UtcNow.UtcDateTime)
            throw new ValidationException(TokenField, "The reset link is invalid or has expired.", ErrorCodes.ResetTokenInvalid);

        var newHash = _passwordHasher.Hash(request.NewPassword);
        await _auth.UpdatePasswordHashAsync(token.UserId, newHash, cancellationToken);
        await _auth.MarkPasswordResetTokenUsedAsync(token.PasswordResetTokenId, cancellationToken);      // single-use
        await _auth.RevokeRefreshTokensAsync(token.UserId, tokenHash: null, allDevices: true, cancellationToken); // BR-AUTH-06
        // TransactionBehavior commits atomically.
    }
}
