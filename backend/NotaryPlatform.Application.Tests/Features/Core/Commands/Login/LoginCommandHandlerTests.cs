using FluentAssertions;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.Commands.Login;
using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Features.Core.Services;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Domain.Features.Core.Enums;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Commands.Login;

/// <summary>
/// Unit tests for <see cref="LoginCommandHandler"/> — UC-AUTH-01 branches plus the UC-AUTH-07 MFA branch
/// (TC-A-01…07). Token issuance is delegated to <see cref="IAuthSessionIssuer"/> (tested separately), so
/// these assert the handler's decisions, not the issuance internals.
/// </summary>
public sealed class LoginCommandHandlerTests
{
    private readonly IAuthRepository _auth = Substitute.For<IAuthRepository>();
    private readonly ILoginAttemptTracker _lockout = Substitute.For<ILoginAttemptTracker>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IMfaRepository _mfa = Substitute.For<IMfaRepository>();
    private readonly IMfaChallengeStore _challengeStore = Substitute.For<IMfaChallengeStore>();
    private readonly IAuthSessionIssuer _sessionIssuer = Substitute.For<IAuthSessionIssuer>();
    private readonly IDateTime _clock = Substitute.For<IDateTime>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 7, 25, 12, 0, 0, TimeSpan.Zero);

    private const string TenantCode = "acme";
    private const string Email = "user@acme.com";
    private const string Password = "correct-horse-battery-staple";
    private const string PasswordHash = "$2a$stored-hash";

    private static readonly AuthSession Session = new(
        "access-token", Now.AddMinutes(60), "RAW-REFRESH", Now.AddDays(30),
        new AuthUserSummary(UserId, TenantId, null, Email, "Test User", new List<string> { "notary" }));

    public LoginCommandHandlerTests() => _clock.UtcNow.Returns(Now);

    private LoginCommandHandler CreateHandler() =>
        new(_auth, _lockout, _passwordHasher, _mfa, _challengeStore, _sessionIssuer, _clock);

    private static LoginCommand Command(string? device = "web") =>
        new(TenantCode, Email, Password, device);

    private static LoginUserRecord ActiveUser(UserStatus status = UserStatus.Active) =>
        new(UserId, TenantId, BranchId: null, Email, DisplayName: "Test User", PasswordHash, status);

    private void ArrangeHappyPath(UserStatus status = UserStatus.Active)
    {
        _auth.FindActiveTenantIdByCodeAsync(TenantCode, Arg.Any<CancellationToken>()).Returns(TenantId);
        _lockout.GetLockoutExpiryAsync(TenantId, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((DateTimeOffset?)null);
        _auth.FindLoginUserAsync(TenantId, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(ActiveUser(status));
        _passwordHasher.Verify(Password, PasswordHash).Returns(true);
        _mfa.HasActiveMfaAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(false);
        _auth.RequiresMfaSetupAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(false);
        _sessionIssuer.IssueAsync(Arg.Any<SessionSubject>(), Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(Session);
        _challengeStore.IssueAsync(Arg.Any<MfaChallengeContext>(), Arg.Any<CancellationToken>())
            .Returns(new MfaChallengeIssued("MFA-CHALLENGE-TOKEN", Now.AddMinutes(5)));
    }

    [Fact] // TC-A-05 — no MFA ⇒ authenticated session (unchanged UC-AUTH-01 behaviour)
    public async Task Valid_credentials_without_mfa_issue_a_session_and_reset_lockout()
    {
        ArrangeHappyPath();

        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        result.Status.Should().Be(LoginResponse.StatusAuthenticated);
        result.Session.Should().Be(Session);
        result.MfaChallenge.Should().BeNull();
        await _sessionIssuer.Received(1).IssueAsync(
            Arg.Is<SessionSubject>(s => s.UserId == UserId && s.TenantId == TenantId), "web", Arg.Any<CancellationToken>());
        await _lockout.Received(1).ResetAsync(TenantId, Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _challengeStore.DidNotReceive().IssueAsync(Arg.Any<MfaChallengeContext>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-A-01/02/03 — MFA user ⇒ mfa_required challenge, NO session, NO challenge-side writes
    public async Task Mfa_enabled_user_gets_a_challenge_and_no_session()
    {
        ArrangeHappyPath();
        _mfa.HasActiveMfaAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(true);

        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        result.Status.Should().Be(LoginResponse.StatusMfaRequired);
        result.Session.Should().BeNull();
        result.MfaChallenge!.MfaToken.Should().Be("MFA-CHALLENGE-TOKEN");
        result.MfaChallenge.Methods.Should().Contain("totp");

        await _challengeStore.Received(1).IssueAsync(
            Arg.Is<MfaChallengeContext>(c => c.UserId == UserId && c.TenantId == TenantId && c.DeviceName == "web"),
            Arg.Any<CancellationToken>());
        await _sessionIssuer.DidNotReceive().IssueAsync(Arg.Any<SessionSubject>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
        // The MFA branch short-circuits the BR-AUTH-05 setup gate.
        await _auth.DidNotReceive().RequiresMfaSetupAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Unknown_tenant_throws_unauthorized_without_looking_up_the_user()
    {
        _auth.FindActiveTenantIdByCodeAsync(TenantCode, Arg.Any<CancellationToken>()).Returns((Guid?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _auth.DidNotReceive().FindLoginUserAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Unknown_user_throws_unauthorized_and_registers_a_failure()
    {
        _auth.FindActiveTenantIdByCodeAsync(TenantCode, Arg.Any<CancellationToken>()).Returns(TenantId);
        _lockout.GetLockoutExpiryAsync(TenantId, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((DateTimeOffset?)null);
        _auth.FindLoginUserAsync(TenantId, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((LoginUserRecord?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _lockout.Received(1).RegisterFailureAsync(TenantId, Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-A-07 — wrong password fails before the MFA decision
    public async Task Wrong_password_throws_unauthorized_registers_failure_and_issues_nothing()
    {
        ArrangeHappyPath();
        _passwordHasher.Verify(Password, PasswordHash).Returns(false);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _lockout.Received(1).RegisterFailureAsync(TenantId, Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _mfa.DidNotReceive().HasActiveMfaAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _sessionIssuer.DidNotReceive().IssueAsync(Arg.Any<SessionSubject>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
        await _challengeStore.DidNotReceive().IssueAsync(Arg.Any<MfaChallengeContext>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Locked_status_throws_account_locked()
    {
        ArrangeHappyPath(UserStatus.Locked);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<AccountLockedException>();
    }

    [Fact] // BR-AUTH-02
    public async Task Active_lockout_window_throws_before_any_credential_check()
    {
        _auth.FindActiveTenantIdByCodeAsync(TenantCode, Arg.Any<CancellationToken>()).Returns(TenantId);
        _lockout.GetLockoutExpiryAsync(TenantId, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Now.AddMinutes(10));

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<AccountLockedException>();
        await _auth.DidNotReceive().FindLoginUserAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        _passwordHasher.DidNotReceive().Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact] // TC-A-06 / BR-AUTH-05 — privileged user with NO device must enrol first
    public async Task Privileged_role_without_mfa_device_throws_forbidden_and_issues_nothing()
    {
        ArrangeHappyPath();
        _mfa.HasActiveMfaAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(false);
        _auth.RequiresMfaSetupAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(true);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
        await _sessionIssuer.DidNotReceive().IssueAsync(Arg.Any<SessionSubject>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
        await _challengeStore.DidNotReceive().IssueAsync(Arg.Any<MfaChallengeContext>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Invited_status_throws_unauthorized()
    {
        ArrangeHappyPath(UserStatus.Invited);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
