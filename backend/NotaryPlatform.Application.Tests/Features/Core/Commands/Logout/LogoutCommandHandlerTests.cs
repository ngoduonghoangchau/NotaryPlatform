using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Caching;
using NotaryPlatform.Application.Features.Core.Commands.Logout;
using NotaryPlatform.Application.Shared.Constants;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Commands.Logout;

/// <summary>Unit tests for UC-AUTH-03's <see cref="LogoutCommandHandler"/> — every branch of the plan §2.</summary>
public sealed class LogoutCommandHandlerTests
{
    private readonly IAuthRepository _auth = Substitute.For<IAuthRepository>();
    private readonly IJwtTokenService _jwt = Substitute.For<IJwtTokenService>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    private const string RawToken = "RAW-REFRESH";
    private const string TokenHash = "HASHED-REFRESH";

    public LogoutCommandHandlerTests()
    {
        _currentUser.UserId.Returns(UserId);
        _currentUser.TenantId.Returns(TenantId);
        _jwt.HashRefreshToken(RawToken).Returns(TokenHash);
    }

    private LogoutCommandHandler CreateHandler() =>
        new(_auth, _jwt, _currentUser, _cache, NullLogger<LogoutCommandHandler>.Instance);

    [Fact]
    public async Task Current_session_logout_revokes_the_presented_token_by_hash()
    {
        await CreateHandler().Handle(new LogoutCommand(RawToken, AllDevices: false), CancellationToken.None);

        await _auth.Received(1).RevokeRefreshTokensAsync(UserId, TokenHash, false, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Global_logout_revokes_all_sessions_and_never_hashes()
    {
        await CreateHandler().Handle(new LogoutCommand(RefreshToken: null, AllDevices: true), CancellationToken.None);

        await _auth.Received(1).RevokeRefreshTokensAsync(UserId, null, true, Arg.Any<CancellationToken>());
        _jwt.DidNotReceive().HashRefreshToken(Arg.Any<string>());
    }

    [Fact]
    public async Task Invalidates_the_users_permission_and_role_cache_keys()
    {
        await CreateHandler().Handle(new LogoutCommand(RawToken, false), CancellationToken.None);

        await _cache.Received(1).RemoveAsync(CacheKeys.UserPermissions(TenantId, UserId), Arg.Any<CancellationToken>());
        await _cache.Received(1).RemoveAsync(CacheKeys.UserRoles(TenantId, UserId), Arg.Any<CancellationToken>());
    }

    [Fact] // fail-soft: a Redis outage must not fail the logout
    public async Task Cache_failure_does_not_fail_the_logout()
    {
        _cache.RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("redis down")));

        var act = async () => await CreateHandler().Handle(new LogoutCommand(RawToken, false), CancellationToken.None);

        await act.Should().NotThrowAsync();
        await _auth.Received(1).RevokeRefreshTokensAsync(UserId, TokenHash, false, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Only_the_hash_reaches_the_repo_never_the_raw_token()
    {
        await CreateHandler().Handle(new LogoutCommand(RawToken, false), CancellationToken.None);

        await _auth.Received(1).RevokeRefreshTokensAsync(
            UserId,
            Arg.Is<string?>(h => h == TokenHash && h != RawToken),
            false,
            Arg.Any<CancellationToken>());
    }
}
