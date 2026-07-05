using FluentAssertions;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Authorization;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.Commands.RefreshToken;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Models.Auth;
using NotaryPlatform.Domain.Features.Core.Enums;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Commands.RefreshToken;

/// <summary>Unit tests for UC-AUTH-02's <see cref="RefreshTokenCommandHandler"/> — every branch of the plan §2.</summary>
public sealed class RefreshTokenCommandHandlerTests
{
    private readonly IAuthRepository _auth = Substitute.For<IAuthRepository>();
    private readonly IJwtTokenService _jwt = Substitute.For<IJwtTokenService>();
    private readonly IPermissionService _permissions = Substitute.For<IPermissionService>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IDateTime _clock = Substitute.For<IDateTime>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid TokenId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 7, 5, 12, 0, 0, TimeSpan.Zero);

    private const string RawToken = "RAW-REFRESH";
    private const string TokenHash = "HASHED-REFRESH";

    public RefreshTokenCommandHandlerTests() => _clock.UtcNow.Returns(Now);

    private RefreshTokenCommandHandler CreateHandler() =>
        new(_auth, _jwt, _permissions, _currentUser, _clock);

    private static RefreshTokenCommand Command() => new(RawToken);

    private static RefreshTokenRecord Token(DateTime? revokedAt = null, DateTime? expiresAt = null) =>
        new(TokenId, TenantId, UserId, expiresAt ?? Now.AddDays(10).UtcDateTime, revokedAt, DeviceName: "web");

    private static ActiveUserRecord User(UserStatus status = UserStatus.Active) =>
        new(UserId, TenantId, BranchId: null, "user@acme.com", "Test User", status);

    private void ArrangeHappyPath()
    {
        _jwt.HashRefreshToken(RawToken).Returns(TokenHash);
        _auth.FindRefreshTokenByHashAsync(TokenHash, Arg.Any<CancellationToken>()).Returns(Token());
        _auth.FindActiveUserByIdAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(User());
        _permissions.GetRolesAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(new List<string> { "notary" });
        _permissions.GetPermissionsAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(new List<string> { "journal.entries.read" });
        _jwt.CreateAccessToken(Arg.Any<JwtTokenClaims>()).Returns(new AccessTokenResult("access-token", Now.AddMinutes(60)));
        _jwt.CreateRefreshToken().Returns("NEW-RAW-REFRESH");
        _jwt.HashRefreshToken("NEW-RAW-REFRESH").Returns("NEW-HASHED-REFRESH");
    }

    [Fact]
    public async Task Valid_token_rotates_and_returns_new_tokens()
    {
        ArrangeHappyPath();

        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("NEW-RAW-REFRESH");               // new raw token to the client
        result.RefreshTokenExpiresAt.Should().Be(Now.AddDays(30));        // BR-AUTH-03 sliding 30-day TTL

        await _auth.Received(1).RotateRefreshTokenAsync(
            TokenId,
            Arg.Is<RefreshTokenCreate>(t => t.TokenHash == "NEW-HASHED-REFRESH"
                                            && t.TokenHash != "NEW-RAW-REFRESH"     // hash persisted, never raw
                                            && t.DeviceName == "web"),              // device carried forward
            Arg.Any<CancellationToken>());
        await _auth.DidNotReceive().RevokeAllRefreshTokensForUserAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Fresh_permission_snapshot_is_resolved_for_the_new_access_token()
    {
        ArrangeHappyPath();

        await CreateHandler().Handle(Command(), CancellationToken.None);

        await _permissions.Received(1).GetRolesAsync(UserId, TenantId, Arg.Any<CancellationToken>());
        await _permissions.Received(1).GetPermissionsAsync(UserId, TenantId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Unknown_token_throws_unauthorized_without_rotating()
    {
        _jwt.HashRefreshToken(RawToken).Returns(TokenHash);
        _auth.FindRefreshTokenByHashAsync(TokenHash, Arg.Any<CancellationToken>()).Returns((RefreshTokenRecord?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _auth.DidNotReceive().RotateRefreshTokenAsync(Arg.Any<Guid>(), Arg.Any<RefreshTokenCreate>(), Arg.Any<CancellationToken>());
    }

    [Fact] // BR-AUTH-04 — reuse of a rotated-out token revokes the whole session
    public async Task Revoked_token_reuse_revokes_entire_session_and_throws()
    {
        _jwt.HashRefreshToken(RawToken).Returns(TokenHash);
        _auth.FindRefreshTokenByHashAsync(TokenHash, Arg.Any<CancellationToken>())
            .Returns(Token(revokedAt: Now.AddMinutes(-5).UtcDateTime));

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _auth.Received(1).RevokeAllRefreshTokensForUserAsync(UserId, Arg.Any<CancellationToken>());
        await _auth.DidNotReceive().RotateRefreshTokenAsync(Arg.Any<Guid>(), Arg.Any<RefreshTokenCreate>(), Arg.Any<CancellationToken>());
    }

    [Fact] // BR-AUTH-03
    public async Task Expired_token_throws_unauthorized_without_rotating_or_revoking()
    {
        _jwt.HashRefreshToken(RawToken).Returns(TokenHash);
        _auth.FindRefreshTokenByHashAsync(TokenHash, Arg.Any<CancellationToken>())
            .Returns(Token(expiresAt: Now.AddMinutes(-1).UtcDateTime));

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _auth.DidNotReceive().RotateRefreshTokenAsync(Arg.Any<Guid>(), Arg.Any<RefreshTokenCreate>(), Arg.Any<CancellationToken>());
        await _auth.DidNotReceive().RevokeAllRefreshTokensForUserAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Inactive_user_throws_unauthorized_without_rotating()
    {
        ArrangeHappyPath();
        _auth.FindActiveUserByIdAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(User(UserStatus.Inactive));

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _auth.DidNotReceive().RotateRefreshTokenAsync(Arg.Any<Guid>(), Arg.Any<RefreshTokenCreate>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Missing_user_throws_unauthorized()
    {
        ArrangeHappyPath();
        _auth.FindActiveUserByIdAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns((ActiveUserRecord?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
