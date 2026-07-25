using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Messaging;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.Commands.InitiatePasswordReset;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Settings;
using NotaryPlatform.Domain.Features.Core.Enums;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Commands.InitiatePasswordReset;

/// <summary>Unit tests for UC-AUTH-05 Step A — <see cref="InitiatePasswordResetCommandHandler"/>.</summary>
public sealed class InitiatePasswordResetCommandHandlerTests
{
    private readonly IAuthRepository _auth = Substitute.For<IAuthRepository>();
    private readonly IResetTokenService _tokens = Substitute.For<IResetTokenService>();
    private readonly IEmailSender _email = Substitute.For<IEmailSender>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IDateTime _clock = Substitute.For<IDateTime>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid AdminId = Guid.NewGuid();
    private static readonly Guid TargetUserId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 7, 24, 12, 0, 0, TimeSpan.Zero);

    private const string RawToken = "RAW-RESET-TOKEN";
    private const string TokenHash = "HASHED-RESET-TOKEN";
    private const string TargetEmail = "target@acme.com";

    public InitiatePasswordResetCommandHandlerTests()
    {
        _currentUser.UserId.Returns(AdminId);
        _currentUser.TenantId.Returns(TenantId);
        _clock.UtcNow.Returns(Now);
        _tokens.CreateResetToken().Returns(RawToken);
        _tokens.HashResetToken(RawToken).Returns(TokenHash);
        _auth.FindActiveUserByIdAsync(TargetUserId, TenantId, Arg.Any<CancellationToken>())
            .Returns(new ActiveUserRecord(TargetUserId, TenantId, null, TargetEmail, "Target User", UserStatus.Active));
    }

    private InitiatePasswordResetCommandHandler CreateHandler() =>
        new(_auth, _tokens, _email, _currentUser, _clock,
            Options.Create(new AppUrls { PublicBaseUrl = "https://app.test" }));

    private static InitiatePasswordResetCommand Command() => new(TargetUserId);

    [Fact]
    public async Task Happy_path_stores_hashed_token_and_emails_link()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        await _auth.Received(1).AddPasswordResetTokenAsync(
            Arg.Is<PasswordResetTokenCreate>(t =>
                t.TokenHash == TokenHash && t.TokenHash != RawToken            // hashed, never raw
                && t.UserId == TargetUserId
                && t.CreatedByUserId == AdminId
                && t.ExpiresAtUtc == Now.AddHours(1).UtcDateTime),             // BR-AUTH-09 (1h)
            Arg.Any<CancellationToken>());

        await _email.Received(1).SendAsync(
            Arg.Is<EmailMessage>(m => m.To == TargetEmail && m.HtmlBody.Contains("token=" + RawToken)),
            Arg.Any<CancellationToken>());
    }

    [Fact] // D-8: one active reset token per user
    public async Task Invalidates_prior_tokens_before_adding_the_new_one()
    {
        await CreateHandler().Handle(Command(), CancellationToken.None);

        Received.InOrder(() =>
        {
            _auth.InvalidatePasswordResetTokensForUserAsync(TargetUserId, Arg.Any<CancellationToken>());
            _auth.AddPasswordResetTokenAsync(Arg.Any<PasswordResetTokenCreate>(), Arg.Any<CancellationToken>());
        });
    }

    [Fact] // BR-AUTH-10 tenant isolation
    public async Task User_not_in_tenant_throws_not_found_and_does_nothing()
    {
        _auth.FindActiveUserByIdAsync(TargetUserId, TenantId, Arg.Any<CancellationToken>())
            .Returns((ActiveUserRecord?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        await _auth.DidNotReceive().AddPasswordResetTokenAsync(Arg.Any<PasswordResetTokenCreate>(), Arg.Any<CancellationToken>());
        await _email.DidNotReceive().SendAsync(Arg.Any<EmailMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Missing_tenant_claim_throws_unauthorized()
    {
        _currentUser.TenantId.Returns((Guid?)null);

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _auth.DidNotReceive().AddPasswordResetTokenAsync(Arg.Any<PasswordResetTokenCreate>(), Arg.Any<CancellationToken>());
    }

    [Fact] // D-3: email failure must propagate so TransactionBehavior rolls back (no orphan token)
    public async Task Email_failure_propagates()
    {
        _email.SendAsync(Arg.Any<EmailMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("smtp down")));

        var act = async () => await CreateHandler().Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
