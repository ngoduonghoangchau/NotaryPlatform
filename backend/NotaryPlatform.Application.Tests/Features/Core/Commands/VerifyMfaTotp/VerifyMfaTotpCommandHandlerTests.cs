using FluentAssertions;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.Commands.VerifyMfaTotp;
using NotaryPlatform.Application.Shared.Constants;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Models.Responses;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Commands.VerifyMfaTotp;

/// <summary>Unit tests for UC-AUTH-06 Step B — <see cref="VerifyMfaTotpCommandHandler"/> (TC-V-01…07).</summary>
public sealed class VerifyMfaTotpCommandHandlerTests
{
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IMfaRepository _mfa = Substitute.For<IMfaRepository>();
    private readonly IMfaSecretVault _vault = Substitute.For<IMfaSecretVault>();
    private readonly ITotpService _totp = Substitute.For<ITotpService>();
    private readonly IRecoveryCodeService _recovery = Substitute.For<IRecoveryCodeService>();
    private readonly IMfaVerifyAttemptTracker _lockout = Substitute.For<IMfaVerifyAttemptTracker>();
    private readonly IDateTime _clock = Substitute.For<IDateTime>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid DeviceId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 7, 25, 12, 0, 0, TimeSpan.Zero);

    private const string SecretRef = "CfDJ8-encrypted-reference";
    private const string RawSecret = "JBSWY3DPEHPK3PXP";
    private const string ValidCode = "123456";

    // Raw recovery codes returned to the user; the handler hashes each before persisting.
    private static readonly IReadOnlyList<string> RawCodes = ["aaaa-bbbb", "cccc-dddd", "eeee-ffff"];

    public VerifyMfaTotpCommandHandlerTests()
    {
        _currentUser.UserId.Returns(UserId);
        _currentUser.TenantId.Returns(TenantId);
        _clock.UtcNow.Returns(Now);
        _lockout.GetLockoutExpiryAsync(UserId, Arg.Any<CancellationToken>()).Returns((DateTimeOffset?)null);
        _mfa.FindPendingByIdAsync(DeviceId, UserId, TenantId, Arg.Any<CancellationToken>())
            .Returns(new PendingMfaDeviceRecord(DeviceId, SecretRef, IsVerified: false));
        _vault.Resolve(SecretRef).Returns(RawSecret);
        _totp.ValidateCode(RawSecret, ValidCode).Returns(true);
        _recovery.Generate(AppDefaults.Security.RecoveryCodeCount).Returns(RawCodes);
        _recovery.Hash(Arg.Any<string>()).Returns(ci => "hash:" + ci.Arg<string>());   // hash != raw
    }

    private VerifyMfaTotpCommandHandler CreateHandler() =>
        new(_currentUser, _mfa, _vault, _totp, _recovery, _lockout, _clock);

    private static VerifyMfaTotpCommand Command(string code = ValidCode) => new(DeviceId, code);

    [Fact] // TC-V-01 — valid code activates + supersedes + issues recovery codes
    public async Task Valid_code_activates_supersedes_and_returns_recovery_codes()
    {
        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        await _mfa.Received(1).ActivateAndSupersedeAsync(DeviceId, UserId, Now.UtcDateTime, Arg.Any<CancellationToken>());
        await _mfa.Received(1).AddRecoveryCodesAsync(UserId, TenantId, Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>());
        result.RecoveryCodes.Should().BeEquivalentTo(RawCodes);
    }

    [Fact] // TC-V-02 — codes persisted are HASHES, not the raw codes returned to the user (Security §5 / D-3)
    public async Task Persists_recovery_code_hashes_not_the_raw_codes()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        var expectedHashes = RawCodes.Select(c => "hash:" + c).ToList();
        await _mfa.Received(1).AddRecoveryCodesAsync(
            UserId,
            TenantId,
            Arg.Is<IReadOnlyList<string>>(h => h.SequenceEqual(expectedHashes) && !h.Any(RawCodes.Contains)),
            Arg.Any<CancellationToken>());
    }

    [Fact] // TC-V-07 — supersede is requested (revoke/demote any prior verified TOTP — one primary, D-5)
    public async Task Activation_supersedes_prior_totp()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        await _mfa.Received(1).ActivateAndSupersedeAsync(DeviceId, UserId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-V-03 / TC-V-06 — wrong code ⇒ 400 MFA_CODE_INVALID, nothing activated or issued
    public async Task Invalid_code_throws_mfa_code_invalid_and_mutates_nothing()
    {
        _totp.ValidateCode(RawSecret, "000000").Returns(false);

        var act = async () => await CreateHandler().Handle(Command("000000"), CancellationToken.None);

        (await act.Should().ThrowAsync<ValidationException>())
            .Which.Errors.Should().Contain(e => e.Code == ErrorCodes.MfaCodeInvalid);
        await _mfa.DidNotReceive().ActivateAndSupersedeAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
        await _mfa.DidNotReceive().AddRecoveryCodesAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-V-03 — a wrong code registers a failure with the rate-limiter (brute-force guard)
    public async Task Invalid_code_registers_a_failed_attempt()
    {
        _totp.ValidateCode(RawSecret, "000000").Returns(false);

        var act = async () => await CreateHandler().Handle(Command("000000"), CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>();

        await _lockout.Received(1).RegisterFailureAsync(UserId, Arg.Any<CancellationToken>());
    }

    [Fact] // TC-V-04 — no such pending device for this user/tenant ⇒ 404
    public async Task Missing_device_throws_not_found()
    {
        _mfa.FindPendingByIdAsync(DeviceId, UserId, TenantId, Arg.Any<CancellationToken>())
            .Returns((PendingMfaDeviceRecord?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        await _mfa.DidNotReceive().ActivateAndSupersedeAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-V-05 — device already verified ⇒ 409, nothing re-issued
    public async Task Already_verified_device_throws_conflict_and_reissues_nothing()
    {
        _mfa.FindPendingByIdAsync(DeviceId, UserId, TenantId, Arg.Any<CancellationToken>())
            .Returns(new PendingMfaDeviceRecord(DeviceId, SecretRef, IsVerified: true));

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
        await _mfa.DidNotReceive().ActivateAndSupersedeAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
        await _mfa.DidNotReceive().AddRecoveryCodesAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-V-06 — secret is resolved and the code checked BEFORE any write
    public async Task Resolves_secret_and_validates_before_any_write()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        Received.InOrder(() =>
        {
            _vault.Resolve(SecretRef);
            _totp.ValidateCode(RawSecret, ValidCode);
            _mfa.ActivateAndSupersedeAsync(DeviceId, UserId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
        });
    }

    [Fact] // §5 hardening — a locked-out user is rejected (423) before any lookup
    public async Task Locked_out_user_is_rejected_before_lookup()
    {
        _lockout.GetLockoutExpiryAsync(UserId, Arg.Any<CancellationToken>()).Returns(Now.AddMinutes(10));

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<AccountLockedException>();
        await _mfa.DidNotReceive().FindPendingByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact] // success clears the failure counter
    public async Task Success_resets_the_attempt_counter()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        await _lockout.Received(1).ResetAsync(UserId, Arg.Any<CancellationToken>());
    }

    [Fact] // missing uid/tid claim ⇒ 401
    public async Task Missing_identity_claim_throws_unauthorized()
    {
        _currentUser.TenantId.Returns((Guid?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
