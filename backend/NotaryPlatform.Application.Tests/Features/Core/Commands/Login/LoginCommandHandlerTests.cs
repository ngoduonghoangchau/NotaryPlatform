using FluentAssertions;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Authorization;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.Commands.Login;
using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Models.Auth;
using NotaryPlatform.Domain.Features.Core.Enums;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Commands.Login;

/// <summary>Unit tests for UC-AUTH-01's <see cref="LoginCommandHandler"/> — every branch of §6 of the plan.</summary>
public sealed class LoginCommandHandlerTests
{
    private readonly IAuthRepository _auth = Substitute.For<IAuthRepository>();
    private readonly ILoginAttemptTracker _lockout = Substitute.For<ILoginAttemptTracker>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IPermissionService _permissions = Substitute.For<IPermissionService>();
    private readonly IJwtTokenService _jwt = Substitute.For<IJwtTokenService>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IDateTime _clock = Substitute.For<IDateTime>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 7, 3, 12, 0, 0, TimeSpan.Zero);

    private const string TenantCode = "acme";
    private const string Email = "user@acme.com";
    private const string Password = "correct-horse-battery-staple";
    private const string PasswordHash = "$2a$stored-hash";

    public LoginCommandHandlerTests() => _clock.UtcNow.Returns(Now);

    private LoginCommandHandler CreateHandler() =>
        new(_auth, _lockout, _passwordHasher, _permissions, _jwt, _currentUser, _clock);

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
        _auth.RequiresMfaSetupAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(false);
        _permissions.GetRolesAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(new List<string> { "notary" });
        _permissions.GetPermissionsAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(new List<string> { "journal.entries.read" });
        _jwt.CreateAccessToken(Arg.Any<JwtTokenClaims>()).Returns(new AccessTokenResult("access-token", Now.AddMinutes(60)));
        _jwt.CreateRefreshToken().Returns("RAW-REFRESH");
        _jwt.HashRefreshToken("RAW-REFRESH").Returns("HASHED-REFRESH");
    }

    [Fact]
    public async Task Valid_credentials_issue_tokens_persist_hash_stamp_login_and_reset_lockout()
    {
        ArrangeHappyPath();

        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("RAW-REFRESH");          // raw token returned to client
        result.User.UserId.Should().Be(UserId);
        result.User.Roles.Should().Contain("notary");

        await _auth.Received(1).AddRefreshTokenAsync(
            Arg.Is<RefreshTokenCreate>(t => t.TokenHash == "HASHED-REFRESH" && t.TokenHash != "RAW-REFRESH"),
            Arg.Any<CancellationToken>());
        await _auth.Received(1).StampLastLoginAsync(UserId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
        await _lockout.Received(1).ResetAsync(TenantId, Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact] // BR-AUTH-07
    public async Task Revokes_prior_device_token_before_adding_the_new_one()
    {
        ArrangeHappyPath();

        await CreateHandler().Handle(Command("web"), CancellationToken.None);

        Received.InOrder(() =>
        {
            _auth.RevokeActiveRefreshTokensForDeviceAsync(UserId, "web", Arg.Any<CancellationToken>());
            _auth.AddRefreshTokenAsync(Arg.Any<RefreshTokenCreate>(), Arg.Any<CancellationToken>());
        });
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

    [Fact]
    public async Task Wrong_password_throws_unauthorized_registers_failure_and_issues_no_token()
    {
        ArrangeHappyPath();
        _passwordHasher.Verify(Password, PasswordHash).Returns(false);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _lockout.Received(1).RegisterFailureAsync(TenantId, Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _auth.DidNotReceive().AddRefreshTokenAsync(Arg.Any<RefreshTokenCreate>(), Arg.Any<CancellationToken>());
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

    [Fact] // BR-AUTH-05 (D2 gate)
    public async Task Privileged_role_without_mfa_throws_forbidden_and_issues_no_token()
    {
        ArrangeHappyPath();
        _auth.RequiresMfaSetupAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(true);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
        await _auth.DidNotReceive().AddRefreshTokenAsync(Arg.Any<RefreshTokenCreate>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Invited_status_throws_unauthorized()
    {
        ArrangeHappyPath(UserStatus.Invited);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
