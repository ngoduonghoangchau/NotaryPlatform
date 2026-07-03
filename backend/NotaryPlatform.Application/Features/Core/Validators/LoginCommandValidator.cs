using FluentValidation;
using NotaryPlatform.Application.Features.Core.Commands.Login;

namespace NotaryPlatform.Application.Features.Core.Validators;

/// <summary>
/// Validates the shape of a login request. Deliberately does NOT enforce password complexity
/// (BR-AUTH-01) — that is a password-creation rule (Change / Reset). Enforcing it at login would
/// reject valid legacy passwords and leak the policy; login only checks that a value is present.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.TenantCode)
            .NotEmpty().WithMessage("Tenant code is required.")
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(320);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.DeviceName)
            .MaximumLength(200);
    }
}
