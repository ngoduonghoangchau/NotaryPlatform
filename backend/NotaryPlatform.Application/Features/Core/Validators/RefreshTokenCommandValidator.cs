using FluentValidation;
using NotaryPlatform.Application.Features.Core.Commands.RefreshToken;

namespace NotaryPlatform.Application.Features.Core.Validators;

/// <summary>
/// Validates the shape of a refresh request. The server-side hash lookup is the real authority on
/// validity, so this only bounds the payload — it deliberately makes no assertion about the token's
/// internal format (which would leak the token's structure).
/// </summary>
public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .MaximumLength(512);
    }
}
