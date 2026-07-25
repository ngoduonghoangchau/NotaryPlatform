using FluentAssertions;
using NotaryPlatform.Application.Features.Core.Commands.VerifyLoginMfa;
using NotaryPlatform.Application.Features.Core.Validators;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Validators;

/// <summary>Unit tests for <see cref="VerifyLoginMfaCommandValidator"/> (TC-VAL-01…03).</summary>
public sealed class VerifyLoginMfaCommandValidatorTests
{
    private readonly VerifyLoginMfaCommandValidator _validator = new();

    [Fact] // TC-VAL-01
    public void Empty_mfa_token_is_invalid()
    {
        var result = _validator.Validate(new VerifyLoginMfaCommand("", "123456"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(VerifyLoginMfaCommand.MfaToken));
    }

    [Theory] // TC-VAL-02
    [InlineData("")]
    [InlineData("   ")]
    public void Empty_code_is_invalid(string code)
    {
        var result = _validator.Validate(new VerifyLoginMfaCommand("MFA-TOKEN", code));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(VerifyLoginMfaCommand.Code));
    }

    [Theory] // TC-VAL-03 — both a TOTP and a recovery code are accepted (the handler decides the method)
    [InlineData("123456")]
    [InlineData("aaaa-bbbb")]
    public void Present_token_and_code_are_valid(string code)
    {
        var result = _validator.Validate(new VerifyLoginMfaCommand("MFA-TOKEN", code));

        result.IsValid.Should().BeTrue();
    }
}
