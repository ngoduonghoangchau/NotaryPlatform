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

    // ── UC-AUTH-08 · conditional fingerprint rule (O-3 / S-1) ─────────────────

    [Fact] // TrustDevice=true requires a fingerprint
    public void Trust_device_without_a_fingerprint_is_invalid()
    {
        var result = _validator.Validate(new VerifyLoginMfaCommand("MFA-TOKEN", "123456", Fingerprint: null, TrustDevice: true));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(VerifyLoginMfaCommand.Fingerprint));
    }

    [Theory] // TrustDevice=true requires a WELL-FORMED fingerprint (S-1 high-entropy format)
    [InlineData("ab")]              // too short (< 8)
    [InlineData("bad fingerprint")] // space not allowed
    public void Trust_device_with_a_malformed_fingerprint_is_invalid(string fingerprint)
    {
        var result = _validator.Validate(new VerifyLoginMfaCommand("MFA-TOKEN", "123456", fingerprint, TrustDevice: true));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(VerifyLoginMfaCommand.Fingerprint));
    }

    [Fact] // TrustDevice=true + valid fingerprint ⇒ valid
    public void Trust_device_with_a_valid_fingerprint_is_valid()
    {
        var result = _validator.Validate(new VerifyLoginMfaCommand("MFA-TOKEN", "123456", "FP-VALID-1234567890", TrustDevice: true));

        result.IsValid.Should().BeTrue();
    }

    [Fact] // O-3 — when NOT trusting, a missing/odd fingerprint is ignored (does not fail a normal MFA login)
    public void Fingerprint_is_ignored_when_not_trusting()
    {
        var result = _validator.Validate(new VerifyLoginMfaCommand("MFA-TOKEN", "123456", Fingerprint: "!!", TrustDevice: false));

        result.IsValid.Should().BeTrue();
    }
}
