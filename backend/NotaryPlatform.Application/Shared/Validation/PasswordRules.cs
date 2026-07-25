using FluentValidation;

namespace NotaryPlatform.Application.Shared.Validation;

/// <summary>
/// Reusable BR-AUTH-01 password-complexity rule, shared by change-password (UC-AUTH-04) and
/// reset-password (UC-AUTH-05): at least 12 characters with an uppercase letter, a lowercase letter,
/// a digit, and a special character.
/// </summary>
public static class PasswordRules
{
    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(12).WithMessage("Password must be at least 12 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.")
            .Matches("[^A-Za-z0-9]").WithMessage("Password must contain a special character.");
}
