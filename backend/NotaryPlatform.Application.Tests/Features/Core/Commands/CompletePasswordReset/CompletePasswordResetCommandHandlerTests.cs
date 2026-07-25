using FluentAssertions;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.Commands.CompletePasswordReset;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Models.Responses;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Commands.CompletePasswordReset;

/// <summary>Unit tests for UC-AUTH-05 Step B — <see cref="CompletePasswordResetCommandHandler"/>.</summary>
public sealed class CompletePasswordResetCommandHandlerTests
{
    private readonly IAuthRepository _auth = Substitute.For<IAuthRepository>();
    private readonly IResetTokenService _tokens = Substitute.For<IResetTokenService>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IDateTime _clock = Substitute.For<IDateTime>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid TokenId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 7, 24, 12, 0, 0, TimeSpan.Zero);

    private const string RawToken = "RAW-RESET-TOKEN";
    private const string TokenHash = "HASHED-RESET-TOKEN";
    private const string NewPassword = "N3w-P@ssw0rd-Str0ng";
    private const string NewHash = "$2a$new-hash";

    public CompletePasswordResetCommandHandlerTests()
    {
        _clock.UtcNow.Returns(Now);
        _tokens.HashResetToken(RawToken).Returns(TokenHash);
        _passwordHasher.Hash(NewPassword).Returns(NewHash);
        _auth.FindPasswordResetTokenByHashAsync(TokenHash, Arg.Any<CancellationToken>())
            .Returns(new PasswordResetTokenRecord(TokenId, TenantId, UserId, Now.AddMinutes(30).UtcDateTime, UsedAtUtc: null));
    }

    private CompletePasswordResetCommandHandler CreateHandler() =>
        new(_auth, _tokens, _passwordHasher, _clock);

    private static CompletePasswordResetCommand Command() => new(RawToken, NewPassword);

    [Fact]
    public async Task Valid_token_sets_password_marks_used_and_revokes_all_sessions()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        await _auth.Received(1).UpdatePasswordHashAsync(UserId, NewHash, Arg.Any<CancellationToken>());
        await _auth.Received(1).MarkPasswordResetTokenUsedAsync(TokenId, Arg.Any<CancellationToken>());          // single-use
        await _auth.Received(1).RevokeRefreshTokensAsync(UserId, null, true, Arg.Any<CancellationToken>());      // BR-AUTH-06
    }

    [Fact]
    public async Task Stores_the_hash_not_the_plaintext()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        await _auth.Received(1).UpdatePasswordHashAsync(
            UserId, Arg.Is<string>(h => h == NewHash && h != NewPassword), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Unknown_token_throws_and_changes_nothing()
    {
        _auth.FindPasswordResetTokenByHashAsync(TokenHash, Arg.Any<CancellationToken>())
            .Returns((PasswordResetTokenRecord?)null);
        await AssertInvalidTokenAndNoMutation();
    }

    [Fact] // BR-AUTH-09 single-use
    public async Task Used_token_throws_and_changes_nothing()
    {
        _auth.FindPasswordResetTokenByHashAsync(TokenHash, Arg.Any<CancellationToken>())
            .Returns(new PasswordResetTokenRecord(TokenId, TenantId, UserId, Now.AddMinutes(30).UtcDateTime, UsedAtUtc: Now.AddMinutes(-5).UtcDateTime));
        await AssertInvalidTokenAndNoMutation();
    }

    [Fact] // BR-AUTH-09 1-hour expiry
    public async Task Expired_token_throws_and_changes_nothing()
    {
        _auth.FindPasswordResetTokenByHashAsync(TokenHash, Arg.Any<CancellationToken>())
            .Returns(new PasswordResetTokenRecord(TokenId, TenantId, UserId, Now.AddMinutes(-1).UtcDateTime, UsedAtUtc: null));
        await AssertInvalidTokenAndNoMutation();
    }

    private async Task AssertInvalidTokenAndNoMutation()
    {
        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        (await act.Should().ThrowAsync<ValidationException>())
            .Which.Errors.Should().Contain(e => e.Code == ErrorCodes.ResetTokenInvalid);   // generic, anti-enumeration
        await _auth.DidNotReceive().UpdatePasswordHashAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _auth.DidNotReceive().MarkPasswordResetTokenUsedAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _auth.DidNotReceive().RevokeRefreshTokensAsync(Arg.Any<Guid>(), Arg.Any<string?>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }
}
