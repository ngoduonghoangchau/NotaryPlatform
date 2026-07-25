using FluentAssertions;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Features.Core.Commands.EnrollMfaTotp;
using NotaryPlatform.Application.Shared.Exceptions;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Commands.EnrollMfaTotp;

/// <summary>Unit tests for UC-AUTH-06 Step A — <see cref="EnrollMfaTotpCommandHandler"/> (TC-E-01…04).</summary>
public sealed class EnrollMfaTotpCommandHandlerTests
{
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly ITotpService _totp = Substitute.For<ITotpService>();
    private readonly IMfaSecretVault _vault = Substitute.For<IMfaSecretVault>();
    private readonly IMfaRepository _mfa = Substitute.For<IMfaRepository>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid NewDeviceId = Guid.NewGuid();

    private const string Email = "jane.doe@acme.com";
    private const string RawSecret = "JBSWY3DPEHPK3PXP";
    private const string VaultRef = "CfDJ8-encrypted-reference";   // ciphertext, NOT the raw secret
    private const string OtpauthUri = "otpauth://totp/NotaryPlatform:jane.doe@acme.com?secret=JBSWY3DPEHPK3PXP&issuer=NotaryPlatform&digits=6&period=30";

    public EnrollMfaTotpCommandHandlerTests()
    {
        _currentUser.UserId.Returns(UserId);
        _currentUser.TenantId.Returns(TenantId);
        _currentUser.Email.Returns(Email);
        _totp.GenerateSecret().Returns(RawSecret);
        _vault.Store(RawSecret).Returns(VaultRef);
        _totp.BuildProvisioningUri(RawSecret, Email, "NotaryPlatform").Returns(OtpauthUri);
        _mfa.AddPendingTotpAsync(Arg.Any<MfaTotpEnrollment>(), Arg.Any<CancellationToken>()).Returns(NewDeviceId);
    }

    private EnrollMfaTotpCommandHandler CreateHandler() => new(_currentUser, _totp, _vault, _mfa);

    private static EnrollMfaTotpCommand Command(string? label = "My Authenticator") => new(label);

    [Fact] // TC-E-01
    public async Task Enroll_generates_secret_stores_it_and_adds_a_pending_totp_row()
    {
        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        _totp.Received(1).GenerateSecret();
        _vault.Received(1).Store(RawSecret);
        await _mfa.Received(1).AddPendingTotpAsync(
            Arg.Is<MfaTotpEnrollment>(e => e.TenantId == TenantId && e.UserId == UserId),
            Arg.Any<CancellationToken>());

        result.MfaDeviceId.Should().Be(NewDeviceId);
        result.OtpauthUri.Should().Contain("otpauth://totp/").And.Contain("issuer=NotaryPlatform");
    }

    [Fact] // TC-E-02 — secret_reference is the vault ref, never the raw secret (Security §5 / D-1)
    public async Task Persists_the_vault_reference_not_the_raw_secret()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        await _mfa.Received(1).AddPendingTotpAsync(
            Arg.Is<MfaTotpEnrollment>(e => e.SecretReference == VaultRef && e.SecretReference != RawSecret),
            Arg.Any<CancellationToken>());
    }

    [Fact] // TC-E-03 — raw secret + URI returned once
    public async Task Returns_the_raw_secret_and_uri_once()
    {
        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        result.Secret.Should().Be(RawSecret);
        result.OtpauthUri.Should().Be(OtpauthUri);
    }

    [Fact] // TC-E-01 (row shape) — the enrollment carries a device code and (optional) label
    public async Task Enrollment_carries_a_device_code_and_the_label()
    {
        await CreateHandler().Handle(Command("My Phone"), CancellationToken.None);

        await _mfa.Received(1).AddPendingTotpAsync(
            Arg.Is<MfaTotpEnrollment>(e => !string.IsNullOrWhiteSpace(e.DeviceCode) && e.Label == "My Phone"),
            Arg.Any<CancellationToken>());
    }

    [Fact] // label trimming / blank → null
    public async Task Blank_label_is_normalized_to_null()
    {
        await CreateHandler().Handle(Command("   "), CancellationToken.None);

        await _mfa.Received(1).AddPendingTotpAsync(
            Arg.Is<MfaTotpEnrollment>(e => e.Label == null),
            Arg.Any<CancellationToken>());
    }

    [Theory] // TC-E-04 — missing uid/tid claim ⇒ 401, nothing stored
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task Missing_identity_claim_throws_unauthorized_and_stores_nothing(bool hasUser, bool hasTenant)
    {
        _currentUser.UserId.Returns(hasUser ? UserId : (Guid?)null);
        _currentUser.TenantId.Returns(hasTenant ? TenantId : (Guid?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _mfa.DidNotReceive().AddPendingTotpAsync(Arg.Any<MfaTotpEnrollment>(), Arg.Any<CancellationToken>());
    }
}
