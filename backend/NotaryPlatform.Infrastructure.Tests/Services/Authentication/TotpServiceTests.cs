using FluentAssertions;
using NotaryPlatform.Infrastructure.Services.Authentication;
using OtpNet;
using Xunit;

namespace NotaryPlatform.Infrastructure.Tests.Services.Authentication;

/// <summary>
/// Real-crypto tests for <see cref="TotpService"/> (Otp.NET, RFC 6238). Covers the integration truth
/// TC-INT-01 (a code computed from the returned secret verifies) and TC-INT-07 (a wrong code does not),
/// without needing a database.
/// </summary>
public sealed class TotpServiceTests
{
    private readonly TotpService _totp = new();

    [Fact]
    public void GenerateSecret_returns_a_decodable_base32_secret_that_differs_each_time()
    {
        var a = _totp.GenerateSecret();
        var b = _totp.GenerateSecret();

        a.Should().NotBeNullOrWhiteSpace();
        a.Should().NotBe(b);
        var act = () => Base32Encoding.ToBytes(a);
        act.Should().NotThrow();   // valid base32
    }

    [Fact] // TC-INT-01 — round trip: generate secret → compute a code → verify
    public void A_code_computed_from_the_secret_validates()
    {
        var secret = _totp.GenerateSecret();
        var code = new Totp(Base32Encoding.ToBytes(secret)).ComputeTotp();

        _totp.ValidateCode(secret, code).Should().BeTrue();
    }

    [Fact] // TC-INT-07 — a wrong code does not validate
    public void A_wrong_code_does_not_validate()
    {
        var secret = _totp.GenerateSecret();
        var correct = new Totp(Base32Encoding.ToBytes(secret)).ComputeTotp();
        var wrong = correct == "000000" ? "111111" : "000000";

        _totp.ValidateCode(secret, wrong).Should().BeFalse();
    }

    [Theory] // robustness — never throws on bad input, just returns false
    [InlineData("", "123456")]
    [InlineData("JBSWY3DPEHPK3PXP", "")]
    [InlineData("not base32 !!!", "123456")]
    [InlineData(null, "123456")]
    public void Malformed_input_returns_false_and_does_not_throw(string? secret, string code)
    {
        _totp.Invoking(t => t.ValidateCode(secret!, code)).Should().NotThrow()
            .Which.Should().BeFalse();
    }

    [Fact]
    public void BuildProvisioningUri_has_the_expected_otpauth_shape()
    {
        var secret = _totp.GenerateSecret();

        var uri = _totp.BuildProvisioningUri(secret, "jane.doe@acme.com", "NotaryPlatform");

        uri.Should().StartWith("otpauth://totp/");
        uri.Should().Contain("issuer=NotaryPlatform");
        uri.Should().Contain($"secret={secret}");
        uri.Should().Contain("digits=6");
        uri.Should().Contain("period=30");
    }
}
