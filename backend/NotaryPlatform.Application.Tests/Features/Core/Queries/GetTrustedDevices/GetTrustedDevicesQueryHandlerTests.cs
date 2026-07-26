using FluentAssertions;
using NSubstitute;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Features.Core.Queries.GetTrustedDevices;
using NotaryPlatform.Application.Shared.Exceptions;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Queries.GetTrustedDevices;

/// <summary>Unit tests for UC-AUTH-08 list — <see cref="GetTrustedDevicesQueryHandler"/> (TC-FUNC-05, TC-AUTHZ-04).</summary>
public sealed class GetTrustedDevicesQueryHandlerTests
{
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly ITrustedDeviceRepository _trustedDevices = Substitute.For<ITrustedDeviceRepository>();

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid DeviceId = Guid.NewGuid();
    private static readonly DateTime TrustedAt = new(2026, 7, 1, 9, 0, 0, DateTimeKind.Utc);

    public GetTrustedDevicesQueryHandlerTests()
    {
        _currentUser.UserId.Returns(UserId);
        _currentUser.TenantId.Returns(TenantId);
    }

    private GetTrustedDevicesQueryHandler CreateHandler() => new(_currentUser, _trustedDevices);

    [Fact] // TC-FUNC-05 / TC-AUTHZ-04 — returns the caller's own devices (scoped to ICurrentUser), mapped 1:1
    public async Task Returns_the_callers_devices_scoped_to_current_user()
    {
        _trustedDevices.ListAsync(UserId, TenantId, Arg.Any<CancellationToken>()).Returns(new List<TrustedDeviceRecord>
        {
            new(DeviceId, "My Laptop", "Windows", TrustedAt, TrustedAt, TrustedAt.AddDays(30), "trusted"),
        });

        var result = await CreateHandler().Handle(new GetTrustedDevicesQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].TrustedDeviceId.Should().Be(DeviceId);
        result[0].DeviceName.Should().Be("My Laptop");
        result[0].ExpiresAtUtc.Should().Be(TrustedAt.AddDays(30));
        result[0].Status.Should().Be("trusted");
        await _trustedDevices.Received(1).ListAsync(UserId, TenantId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Missing_identity_claim_throws_unauthorized()
    {
        _currentUser.UserId.Returns((Guid?)null);

        var act = async () => await CreateHandler().Handle(new GetTrustedDevicesQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        await _trustedDevices.DidNotReceive().ListAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
