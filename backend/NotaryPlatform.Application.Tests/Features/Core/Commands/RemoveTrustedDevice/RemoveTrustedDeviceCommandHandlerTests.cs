using FluentAssertions;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.Commands.RemoveTrustedDevice;
using NotaryPlatform.Application.Shared.Exceptions;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Commands.RemoveTrustedDevice;

/// <summary>Unit tests for UC-AUTH-08 remove — <see cref="RemoveTrustedDeviceCommandHandler"/> (TC-FUNC-06, TC-AUTHZ-03, TC-NEG-04).</summary>
public sealed class RemoveTrustedDeviceCommandHandlerTests
{
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly ITrustedDeviceRepository _trustedDevices = Substitute.For<ITrustedDeviceRepository>();
    private readonly IDateTime _clock = Substitute.For<IDateTime>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid DeviceId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 7, 25, 12, 0, 0, TimeSpan.Zero);

    public RemoveTrustedDeviceCommandHandlerTests()
    {
        _currentUser.UserId.Returns(UserId);
        _currentUser.TenantId.Returns(TenantId);
        _clock.UtcNow.Returns(Now);
    }

    private RemoveTrustedDeviceCommandHandler CreateHandler() => new(_currentUser, _trustedDevices, _clock);

    [Fact] // TC-FUNC-06 — a caller's own device is soft-revoked, scoped to (user, tenant)
    public async Task Revokes_the_callers_own_device()
    {
        _trustedDevices.RevokeAsync(DeviceId, UserId, TenantId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(true);

        await CreateHandler().Handle(new RemoveTrustedDeviceCommand(DeviceId), CancellationToken.None);

        await _trustedDevices.Received(1).RevokeAsync(DeviceId, UserId, TenantId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact] // TC-AUTHZ-03 / TC-NEG-04 — a device that is not the caller's (or does not exist) ⇒ 404 (anti-enumeration)
    public async Task Not_owned_or_missing_device_throws_not_found()
    {
        _trustedDevices.RevokeAsync(DeviceId, UserId, TenantId, Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await CreateHandler().Handle(new RemoveTrustedDeviceCommand(DeviceId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Missing_identity_claim_throws_unauthorized()
    {
        _currentUser.TenantId.Returns((Guid?)null);

        var act = async () => await CreateHandler().Handle(new RemoveTrustedDeviceCommand(DeviceId), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _trustedDevices.DidNotReceive().RevokeAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }
}
