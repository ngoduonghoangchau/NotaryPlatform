using FluentValidation;
using NotaryPlatform.Application.Features.Core.Commands.Logout;

namespace NotaryPlatform.Application.Features.Core.Validators;

/// <summary>
/// Validates the logout request shape. A refresh token names the current session, so it is required
/// unless <c>AllDevices</c> is set (which revokes every session and needs no single token). No format
/// assertion beyond a length bound — the server-side hash lookup is the authority.
/// </summary>
public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .When(x => !x.AllDevices);

        RuleFor(x => x.RefreshToken)
            .MaximumLength(512)
            .When(x => !string.IsNullOrEmpty(x.RefreshToken));
    }
}
