using FluentValidation;
using NotaryPlatform.Application.Features.Core.Commands.ChangePassword;
using NotaryPlatform.Application.Shared.Validation;

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
            .Password()   // BR-AUTH-01 (shared with UC-AUTH-05)
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must differ from the current password.");
    }
}
