using FluentAssertions;
using NotaryPlatform.Application.Features.Core.Commands.VerifyMfaTotp;
using NotaryPlatform.Application.Features.Core.Validators;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Validators;

/// <summary>Unit tests for <see cref="VerifyMfaTotpCommandValidator"/> (TC-VAL-02…04).</summary>
public sealed class VerifyMfaTotpCommandValidatorTests
{
    private readonly VerifyMfaTotpCommandValidator _validator = new();

    private static readonly Guid DeviceId = Guid.NewGuid();

    [Fact] // TC-VAL-02 — empty device id is invalid
    public void Empty_device_id_is_invalid()
    {
        var result = _validator.Validate(new VerifyMfaTotpCommand(Guid.Empty, "123456"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(VerifyMfaTotpCommand.MfaDeviceId));
    }

    [Theory] // TC-VAL-03 — code must be EXACTLY 6 digits
    [InlineData("12345")]     // too short
    [InlineData("1234567")]   // too long
    [InlineData("12a456")]    // non-digit
    [InlineData("")]          // empty
    [InlineData("   ")]       // whitespace
    public void Code_that_is_not_six_digits_is_invalid(string code)
    {
        var result = _validator.Validate(new VerifyMfaTotpCommand(DeviceId, code));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(VerifyMfaTotpCommand.Code));
    }

    [Fact] // TC-VAL-04 — a 6-digit code + a valid device id is valid
    public void Six_digit_code_with_device_id_is_valid()
    {
        var result = _validator.Validate(new VerifyMfaTotpCommand(DeviceId, "123456"));

        result.IsValid.Should().BeTrue();
    }
}
