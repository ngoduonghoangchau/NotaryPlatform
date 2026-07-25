using FluentAssertions;
using NotaryPlatform.Application.Features.Core.Commands.EnrollMfaTotp;
using NotaryPlatform.Application.Features.Core.Validators;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Validators;

/// <summary>Unit tests for <see cref="EnrollMfaTotpCommandValidator"/> (TC-VAL-01).</summary>
public sealed class EnrollMfaTotpCommandValidatorTests
{
    private readonly EnrollMfaTotpCommandValidator _validator = new();

    [Fact] // TC-VAL-01 — label longer than 200 is invalid
    public void Label_over_200_chars_is_invalid()
    {
        var result = _validator.Validate(new EnrollMfaTotpCommand(new string('a', 201)));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(EnrollMfaTotpCommand.Label));
    }

    [Fact]
    public void Label_at_200_chars_is_valid()
    {
        var result = _validator.Validate(new EnrollMfaTotpCommand(new string('a', 200)));

        result.IsValid.Should().BeTrue();
    }

    [Fact] // label is optional
    public void Null_label_is_valid()
    {
        var result = _validator.Validate(new EnrollMfaTotpCommand(null));

        result.IsValid.Should().BeTrue();
    }
}
