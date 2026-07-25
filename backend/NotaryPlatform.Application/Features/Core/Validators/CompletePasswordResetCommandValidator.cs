using FluentValidation;
using NotaryPlatform.Application.Features.Core.Commands.CompletePasswordReset;
using NotaryPlatform.Application.Shared.Validation;

namespace NotaryPlatform.Application.Features.Core.Validators;

/// <summary>Validates the reset-complete request: a present token + a BR-AUTH-01-compliant new password.</summary>
public sealed class CompletePasswordResetCommandValidator : AbstractValidator<CompletePasswordResetCommand>
{
    public CompletePasswordResetCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Reset token is required.")
            .MaximumLength(512);

        RuleFor(x => x.NewPassword).Password();   // BR-AUTH-01
    }
}
