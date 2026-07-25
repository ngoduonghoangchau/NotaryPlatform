using FluentValidation;
using NotaryPlatform.Application.Features.Core.Commands.InitiatePasswordReset;

namespace NotaryPlatform.Application.Features.Core.Validators;

/// <summary>Validates the admin-initiate request shape. The target tenant comes from the JWT, never the body.</summary>
public sealed class InitiatePasswordResetCommandValidator : AbstractValidator<InitiatePasswordResetCommand>
{
    public InitiatePasswordResetCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User id is required.");
    }
}
