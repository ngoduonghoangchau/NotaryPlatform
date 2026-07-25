using FluentAssertions;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Authorization;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.Services;
using NotaryPlatform.Application.Shared.Models.Auth;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Services;

/// <summary>Unit tests for the shared <see cref="AuthSessionIssuer"/> (TC-S-01…03) used by both login paths.</summary>
public sealed class AuthSessionIssuerTests
{
    private readonly IAuthRepository _auth = Substitute.For<IAuthRepository>();
    private readonly IPermissionService _permissions = Substitute.For<IPermissionService>();
    private readonly IJwtTokenService _jwt = Substitute.For<IJwtTokenService>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IDateTime _clock = Substitute.For<IDateTime>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 7, 25, 12, 0, 0, TimeSpan.Zero);

    private const string Email = "user@acme.com";

    public AuthSessionIssuerTests()
    {
        _clock.UtcNow.Returns(Now);
        _permissions.GetRolesAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(new List<string> { "notary" });
        _permissions.GetPermissionsAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(new List<string> { "journal.entries.read" });
        _jwt.CreateAccessToken(Arg.Any<JwtTokenClaims>()).Returns(new AccessTokenResult("access-token", Now.AddMinutes(60)));
        _jwt.CreateRefreshToken().Returns("RAW-REFRESH");
        _jwt.HashRefreshToken("RAW-REFRESH").Returns("HASHED-REFRESH");
    }

    private AuthSessionIssuer CreateIssuer() => new(_auth, _permissions, _jwt, _currentUser, _clock);

    private static SessionSubject Subject() => new(UserId, TenantId, BranchId: null, Email, "Test User");

    [Fact] // TC-S-01 — the refresh token is stored HASHED, never raw
    public async Task Issues_tokens_and_persists_the_hashed_refresh_token()
    {
        var session = await CreateIssuer().IssueAsync(Subject(), "web", CancellationToken.None);

        session.AccessToken.Should().Be("access-token");
        session.RefreshToken.Should().Be("RAW-REFRESH");   // raw returned to the client once
        session.User.UserId.Should().Be(UserId);
        session.User.Roles.Should().Contain("notary");

        await _auth.Received(1).AddRefreshTokenAsync(
            Arg.Is<RefreshTokenCreate>(t => t.TokenHash == "HASHED-REFRESH" && t.TokenHash != "RAW-REFRESH"),
            Arg.Any<CancellationToken>());
    }

    [Fact] // TC-S-02 / BR-AUTH-07 — revoke the prior device token before adding the new one
    public async Task Revokes_prior_device_token_before_adding_the_new_one()
    {
        await CreateIssuer().IssueAsync(Subject(), "web", CancellationToken.None);

        Received.InOrder(() =>
        {
            _auth.RevokeActiveRefreshTokensForDeviceAsync(UserId, "web", Arg.Any<CancellationToken>());
            _auth.AddRefreshTokenAsync(Arg.Any<RefreshTokenCreate>(), Arg.Any<CancellationToken>());
        });
    }

    [Fact] // TC-S-03 — last-login stamped
    public async Task Stamps_last_login()
    {
        await CreateIssuer().IssueAsync(Subject(), "web", CancellationToken.None);

        await _auth.Received(1).StampLastLoginAsync(UserId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }
}
