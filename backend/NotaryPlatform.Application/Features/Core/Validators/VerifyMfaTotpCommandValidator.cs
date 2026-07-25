using FluentValidation;
using NotaryPlatform.Application.Features.Core.Commands.VerifyMfaTotp;

namespace NotaryPlatform.Application.Features.Core.Validators;

/// <summary>Validates TOTP verification: a non-empty device id and an exactly-6-digit code.</summary>
public sealed class VerifyMfaTotpCommandValidator : AbstractValidator<VerifyMfaTotpCommand>
{
    public VerifyMfaTotpCommandValidator()
    {
        RuleFor(x => x.MfaDeviceId)
            .NotEmpty().WithMessage("A device id is required.");

        RuleFor(x => x.Code)
            .Matches("^\\d{6}$").WithMessage("A 6-digit code is required.");
    }
}
