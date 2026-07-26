using FluentValidation;
using NotaryPlatform.Application.Features.Core.Commands.RemoveTrustedDevice;

namespace NotaryPlatform.Application.Features.Core.Validators;

/// <summary>Validates the trusted-device removal request: a non-empty device id.</summary>
public sealed class RemoveTrustedDeviceCommandValidator : AbstractValidator<RemoveTrustedDeviceCommand>
{
    public RemoveTrustedDeviceCommandValidator()
    {
        RuleFor(x => x.TrustedDeviceId)
            .NotEmpty().WithMessage("A trusted device id is required.");
    }
}
