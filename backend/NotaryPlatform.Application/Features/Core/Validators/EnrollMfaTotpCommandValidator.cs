using FluentValidation;
using NotaryPlatform.Application.Features.Core.Commands.EnrollMfaTotp;

namespace NotaryPlatform.Application.Features.Core.Validators;

/// <summary>Validates TOTP enrollment: an optional label bounded to the column length (200).</summary>
public sealed class EnrollMfaTotpCommandValidator : AbstractValidator<EnrollMfaTotpCommand>
{
    public EnrollMfaTotpCommandValidator()
    {
        RuleFor(x => x.Label)
            .MaximumLength(200).WithMessage("Label must be 200 characters or fewer.")
            .When(x => x.Label is not null);
    }
}
