using FluentValidation;
using NotaryPlatform.Application.Features.Core.Commands.VerifyLoginMfa;

namespace NotaryPlatform.Application.Features.Core.Validators;

/// <summary>
/// Validates the MFA-login-verify request: a present challenge token and a present code. The code carries
/// EITHER a 6-digit TOTP or an <c>xxxx-xxxx</c> recovery code, so this only bounds it (the handler decides
/// the method); a wrong-but-well-formed code is a <c>MFA_CODE_INVALID</c>, not a validation error.
/// </summary>
public sealed class VerifyLoginMfaCommandValidator : AbstractValidator<VerifyLoginMfaCommand>
{
    public VerifyLoginMfaCommandValidator()
    {
        RuleFor(x => x.MfaToken)
            .NotEmpty().WithMessage("An MFA challenge token is required.")
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("A verification code is required.")
            .MaximumLength(32);
    }
}
