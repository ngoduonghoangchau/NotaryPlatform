using FluentAssertions;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Features.Core.Commands.ChangePassword;
using NotaryPlatform.Application.Shared.Exceptions;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Commands.ChangePassword;

/// <summary>Unit tests for UC-AUTH-04's <see cref="ChangePasswordCommandHandler"/> — every branch of the plan §2.</summary>
public sealed class ChangePasswordCommandHandlerTests
{
    private readonly IAuthRepository _auth = Substitute.For<IAuthRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    private const string CurrentPassword = "OldP@ssw0rd123";
    private const string NewPassword = "N3w-P@ssw0rd-Str0ng";
    private const string StoredHash = "$2a$stored-hash";
    private const string NewHash = "$2a$new-hash";

    public ChangePasswordCommandHandlerTests()
    {
        _currentUser.UserId.Returns(UserId);
        _currentUser.TenantId.Returns(TenantId);
        _auth.FindPasswordHashAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(StoredHash);
        _passwordHasher.Verify(CurrentPassword, StoredHash).Returns(true);
        _passwordHasher.Hash(NewPassword).Returns(NewHash);
    }

    private ChangePasswordCommandHandler CreateHandler() =>
        new(_auth, _passwordHasher, _currentUser);

    private static ChangePasswordCommand Command() => new(CurrentPassword, NewPassword);

    [Fact]
    public async Task Valid_change_updates_hash_and_revokes_all_sessions()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        await _auth.Received(1).UpdatePasswordHashAsync(UserId, NewHash, Arg.Any<CancellationToken>());
        await _auth.Received(1).RevokeRefreshTokensAsync(UserId, null, true, Arg.Any<CancellationToken>()); // BR-AUTH-06
    }

    [Fact]
    public async Task Stores_the_hash_not_the_plaintext_new_password()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        await _auth.Received(1).UpdatePasswordHashAsync(
            UserId,
            Arg.Is<string>(h => h == NewHash && h != NewPassword),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Verifies_current_password_before_hashing_the_new_one()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        Received.InOrder(() =>
        {
            _passwordHasher.Verify(CurrentPassword, StoredHash);
            _passwordHasher.Hash(NewPassword);
        });
    }

    [Fact]
    public async Task Wrong_current_password_throws_validation_and_changes_nothing()
    {
        _passwordHasher.Verify(CurrentPassword, StoredHash).Returns(false);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        (await act.Should().ThrowAsync<ValidationException>())
            .Which.Errors.Should().Contain(e => e.Field == "currentPassword");
        await _auth.DidNotReceive().UpdatePasswordHashAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _auth.DidNotReceive().RevokeRefreshTokensAsync(Arg.Any<Guid>(), Arg.Any<string?>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Missing_user_throws_unauthorized_and_changes_nothing()
    {
        _auth.FindPasswordHashAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns((string?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _auth.DidNotReceive().UpdatePasswordHashAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact] // a validly-signed token missing the tid claim must 401, not crash with a 500
    public async Task Missing_tenant_claim_throws_unauthorized_and_changes_nothing()
    {
        _currentUser.TenantId.Returns((Guid?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _auth.DidNotReceive().UpdatePasswordHashAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
