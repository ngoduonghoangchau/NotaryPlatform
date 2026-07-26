using FluentAssertions;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.Commands.VerifyLoginMfa;
using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Features.Core.Services;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Models.Responses;
using NotaryPlatform.Domain.Features.Core.Enums;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Commands.VerifyLoginMfa;

/// <summary>Unit tests for UC-AUTH-07 Step B — <see cref="VerifyLoginMfaCommandHandler"/> (TC-B-01…13).</summary>
public sealed class VerifyLoginMfaCommandHandlerTests
{
    private readonly IMfaChallengeStore _challengeStore = Substitute.For<IMfaChallengeStore>();
    private readonly IMfaVerifyAttemptTracker _lockout = Substitute.For<IMfaVerifyAttemptTracker>();
    private readonly IAuthRepository _auth = Substitute.For<IAuthRepository>();
    private readonly IMfaRepository _mfa = Substitute.For<IMfaRepository>();
    private readonly IMfaSecretVault _vault = Substitute.For<IMfaSecretVault>();
    private readonly ITotpService _totp = Substitute.For<ITotpService>();
    private readonly IRecoveryCodeService _recovery = Substitute.For<IRecoveryCodeService>();
    private readonly ITrustedDeviceRepository _trustedDevices = Substitute.For<ITrustedDeviceRepository>();
    private readonly IAuthSessionIssuer _sessionIssuer = Substitute.For<IAuthSessionIssuer>();
    private readonly IDateTime _clock = Substitute.For<IDateTime>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid DeviceId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 7, 25, 12, 0, 0, TimeSpan.Zero);

    private const string MfaToken = "MFA-CHALLENGE-TOKEN";
    private const string SecretRef = "CfDJ8-encrypted-reference";
    private const string RawSecret = "JBSWY3DPEHPK3PXP";
    private const string TotpCode = "123456";
    private const string RecoveryCode = "aaaa-bbbb";
    private const string Email = "user@acme.com";

    private static readonly AuthSession Session = new(
        "access-token", Now.AddMinutes(60), "RAW-REFRESH", Now.AddDays(30),
        new AuthUserSummary(UserId, TenantId, null, Email, "Test User", new List<string> { "notary" }));

    public VerifyLoginMfaCommandHandlerTests()
    {
        _clock.UtcNow.Returns(Now);
        _challengeStore.ResolveAsync(MfaToken, Arg.Any<CancellationToken>())
            .Returns(new MfaChallengeContext(UserId, TenantId, "web"));
        _lockout.GetLockoutExpiryAsync(UserId, Arg.Any<CancellationToken>()).Returns((DateTimeOffset?)null);
        _auth.FindActiveUserByIdAsync(UserId, TenantId, Arg.Any<CancellationToken>())
            .Returns(new ActiveUserRecord(UserId, TenantId, null, Email, "Test User", UserStatus.Active));
        _mfa.FindActiveTotpAsync(UserId, TenantId, Arg.Any<CancellationToken>())
            .Returns(new TotpChallengeRecord(DeviceId, SecretRef));
        _vault.Resolve(SecretRef).Returns(RawSecret);
        _totp.ValidateCode(RawSecret, TotpCode).Returns(true);
        _recovery.Hash(Arg.Any<string>()).Returns(ci => "hash:" + ci.Arg<string>());
        _mfa.TryConsumeRecoveryCodeAsync(UserId, TenantId, "hash:" + RecoveryCode, Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(true);
        _sessionIssuer.IssueAsync(Arg.Any<SessionSubject>(), Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(Session);
    }

    private VerifyLoginMfaCommandHandler CreateHandler() =>
        new(_challengeStore, _lockout, _auth, _mfa, _vault, _totp, _recovery, _trustedDevices, _sessionIssuer, _clock);

    private static VerifyLoginMfaCommand Command(string code = TotpCode) => new(MfaToken, code);

    [Fact] // TC-B-01 — valid TOTP ⇒ authenticated session
    public async Task Valid_totp_issues_a_session()
    {
        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        result.Status.Should().Be(LoginResponse.StatusAuthenticated);
        result.Session.Should().Be(Session);
        await _sessionIssuer.Received(1).IssueAsync(
            Arg.Is<SessionSubject>(s => s.UserId == UserId && s.TenantId == TenantId), "web", Arg.Any<CancellationToken>());
    }

    [Fact] // TC-B-02 — success consumes the challenge (single-use) and stamps the device
    public async Task Valid_totp_invalidates_the_challenge_and_stamps_the_device()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        await _challengeStore.Received(1).InvalidateAsync(MfaToken, Arg.Any<CancellationToken>());
        await _mfa.Received(1).StampDeviceUsedAsync(DeviceId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-B-03 — success clears the per-user MFA counter
    public async Task Valid_totp_resets_the_attempt_counter()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        await _lockout.Received(1).ResetAsync(UserId, Arg.Any<CancellationToken>());
    }

    [Fact] // TC-B-04/05 — valid recovery code ⇒ session; consumed by HASH (single-use); TOTP path not taken
    public async Task Valid_recovery_code_consumes_it_and_issues_a_session()
    {
        var result = await CreateHandler().Handle(Command(RecoveryCode), CancellationToken.None);

        result.Status.Should().Be(LoginResponse.StatusAuthenticated);
        _recovery.Received(1).Hash(RecoveryCode);
        await _mfa.Received(1).TryConsumeRecoveryCodeAsync(
            UserId, TenantId, Arg.Is<string>(h => h == "hash:" + RecoveryCode && h != RecoveryCode), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
        await _mfa.DidNotReceive().FindActiveTotpAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _sessionIssuer.Received(1).IssueAsync(Arg.Any<SessionSubject>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-B-06/13 — wrong TOTP ⇒ 400 MFA_CODE_INVALID, no session
    public async Task Wrong_totp_throws_code_invalid_and_issues_no_session()
    {
        _totp.ValidateCode(RawSecret, "000000").Returns(false);

        var act = async () => await CreateHandler().Handle(Command("000000"), CancellationToken.None);

        (await act.Should().ThrowAsync<ValidationException>())
            .Which.Errors.Should().Contain(e => e.Code == ErrorCodes.MfaCodeInvalid);
        await _sessionIssuer.DidNotReceive().IssueAsync(Arg.Any<SessionSubject>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-B-07 — invalid code keeps the challenge alive and registers a failure
    public async Task Wrong_code_does_not_consume_the_challenge_and_registers_a_failure()
    {
        _totp.ValidateCode(RawSecret, "000000").Returns(false);

        var act = async () => await CreateHandler().Handle(Command("000000"), CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>();

        await _challengeStore.DidNotReceive().InvalidateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _lockout.Received(1).RegisterFailureAsync(UserId, Arg.Any<CancellationToken>());
    }

    [Fact] // TC-B-08 — wrong recovery code ⇒ 400, no session, challenge survives
    public async Task Wrong_recovery_code_throws_code_invalid()
    {
        // TryConsume returns false (default) for an unmatched hash.
        var act = async () => await CreateHandler().Handle(Command("zzzz-zzzz"), CancellationToken.None);

        (await act.Should().ThrowAsync<ValidationException>())
            .Which.Errors.Should().Contain(e => e.Code == ErrorCodes.MfaCodeInvalid);
        await _sessionIssuer.DidNotReceive().IssueAsync(Arg.Any<SessionSubject>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
        await _challengeStore.DidNotReceive().InvalidateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-B-09/10 — unknown/expired/consumed challenge ⇒ 401 MFA_CHALLENGE_INVALID
    public async Task Unknown_challenge_throws_challenge_invalid()
    {
        _challengeStore.ResolveAsync(MfaToken, Arg.Any<CancellationToken>()).Returns((MfaChallengeContext?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        (await act.Should().ThrowAsync<UnauthorizedException>())
            .Which.ErrorCode.Should().Be(ErrorCodes.MfaChallengeInvalid);
        await _sessionIssuer.DidNotReceive().IssueAsync(Arg.Any<SessionSubject>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-B-11 — locked ⇒ 423, challenge invalidated, code never validated
    public async Task Locked_out_user_is_rejected_and_challenge_invalidated()
    {
        _lockout.GetLockoutExpiryAsync(UserId, Arg.Any<CancellationToken>()).Returns(Now.AddMinutes(10));

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<AccountLockedException>();
        await _challengeStore.Received(1).InvalidateAsync(MfaToken, Arg.Any<CancellationToken>());
        _totp.DidNotReceive().ValidateCode(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact] // TC-B-12 — user deactivated between step A and B ⇒ 401
    public async Task Deactivated_user_between_steps_throws_unauthorized()
    {
        _auth.FindActiveUserByIdAsync(UserId, TenantId, Arg.Any<CancellationToken>())
            .Returns(new ActiveUserRecord(UserId, TenantId, null, Email, "Test User", UserStatus.Locked));

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _sessionIssuer.DidNotReceive().IssueAsync(Arg.Any<SessionSubject>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-B-12 — user vanished ⇒ 401
    public async Task Missing_user_between_steps_throws_unauthorized()
    {
        _auth.FindActiveUserByIdAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns((ActiveUserRecord?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    // ── UC-AUTH-08 · trust-this-device opt-in ─────────────────────────────────

    private const string ValidFingerprint = "FP-VALID-1234567890";

    [Fact] // TC-FUNC-01 — trustDevice=true registers the device after a proven MFA
    public async Task Trust_opt_in_registers_the_device_on_success()
    {
        var command = new VerifyLoginMfaCommand(MfaToken, TotpCode, ValidFingerprint, TrustDevice: true, DeviceName: "My Laptop");

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.Status.Should().Be(LoginResponse.StatusAuthenticated);
        await _trustedDevices.Received(1).TrustDeviceAsync(
            Arg.Is<TrustedDeviceRegistration>(r => r.UserId == UserId && r.TenantId == TenantId && r.Fingerprint == ValidFingerprint),
            Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-FUNC-02 — trustDevice=false does not register
    public async Task No_trust_opt_in_does_not_register()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);   // TrustDevice defaults false

        await _trustedDevices.DidNotReceive().TrustDeviceAsync(Arg.Any<TrustedDeviceRegistration>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-NEG-05 / O-5 — a fingerprint owned by another user ⇒ 409, challenge NOT consumed, no session
    public async Task Trust_conflict_throws_409_and_preserves_the_challenge()
    {
        _trustedDevices.TrustDeviceAsync(Arg.Any<TrustedDeviceRegistration>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(TrustDeviceOutcome.ConflictDifferentUser);

        var command = new VerifyLoginMfaCommand(MfaToken, TotpCode, ValidFingerprint, TrustDevice: true);
        var act = async () => await CreateHandler().Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
        await _challengeStore.DidNotReceive().InvalidateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _sessionIssuer.DidNotReceive().IssueAsync(Arg.Any<SessionSubject>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-MFA-01 — a wrong code never trusts the device (registration only after a proven MFA)
    public async Task Wrong_code_with_trust_opt_in_does_not_register()
    {
        _totp.ValidateCode(RawSecret, "000000").Returns(false);

        var command = new VerifyLoginMfaCommand(MfaToken, "000000", ValidFingerprint, TrustDevice: true);
        var act = async () => await CreateHandler().Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        await _trustedDevices.DidNotReceive().TrustDeviceAsync(Arg.Any<TrustedDeviceRegistration>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-MFA-02 — a recovery-code login can also trust the device
    public async Task Recovery_code_login_can_trust_the_device()
    {
        var command = new VerifyLoginMfaCommand(MfaToken, RecoveryCode, ValidFingerprint, TrustDevice: true);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.Status.Should().Be(LoginResponse.StatusAuthenticated);
        await _trustedDevices.Received(1).TrustDeviceAsync(
            Arg.Is<TrustedDeviceRegistration>(r => r.Fingerprint == ValidFingerprint), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }
}
