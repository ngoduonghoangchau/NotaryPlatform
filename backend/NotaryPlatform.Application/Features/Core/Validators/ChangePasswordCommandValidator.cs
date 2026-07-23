using FluentValidation;
using NotaryPlatform.Application.Features.Core.Commands.ChangePassword;

namespace NotaryPlatform.Application.Features.Core.Validators;

/// <summary>
/// Enforces BR-AUTH-01 password complexity on the new password — this is the first place it is
/// enforced (login deliberately skips it, as it is a password-creation rule). Also requires the new
/// password to differ from the current one. These rules are reusable by UC-AUTH-05 (admin reset);
/// extract them to a shared rule set when it lands.
/// </summary>
public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(12).WithMessage("Password must be at least 12 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.")
            .Matches("[^A-Za-z0-9]").WithMessage("Password must contain a special character.")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must differ from the current password.");
    }
}
