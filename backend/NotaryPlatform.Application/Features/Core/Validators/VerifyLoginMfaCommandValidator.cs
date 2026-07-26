using FluentValidation;
using NotaryPlatform.Application.Features.Core.Commands.VerifyLoginMfa;
using NotaryPlatform.Application.Shared.Constants;

namespace NotaryPlatform.Application.Features.Core.Validators;

/// <summary>
/// Validates the MFA-login-verify request: a present challenge token and a present code. The code carries
/// EITHER a 6-digit TOTP or an <c>xxxx-xxxx</c> recovery code, so this only bounds it (the handler decides
/// the method); a wrong-but-well-formed code is a <c>MFA_CODE_INVALID</c>, not a validation error.
///
/// UC-AUTH-08 — when the caller opts to trust the device (<c>TrustDevice = true</c>), a well-formed
/// fingerprint is required (S-1: high-entropy format); otherwise the fingerprint is ignored (O-3).
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

        // Trusting a device requires a valid, high-entropy fingerprint (S-1). When TrustDevice is false the
        // fingerprint is unused, so it is not validated (a stray value never fails a normal MFA login).
        When(x => x.TrustDevice, () =>
        {
            RuleFor(x => x.Fingerprint)
                .NotEmpty().WithMessage("A device fingerprint is required to trust this device.")
                .Matches(AppDefaults.Security.DeviceFingerprintPattern).WithMessage("The device fingerprint format is invalid.");
        });

        RuleFor(x => x.DeviceName).MaximumLength(200).When(x => x.DeviceName is not null);
        RuleFor(x => x.Platform).MaximumLength(100).When(x => x.Platform is not null);
        RuleFor(x => x.Browser).MaximumLength(100).When(x => x.Browser is not null);
    }
}
