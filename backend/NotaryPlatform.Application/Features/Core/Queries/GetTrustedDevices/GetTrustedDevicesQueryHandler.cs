using MediatR;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Exceptions;

namespace NotaryPlatform.Application.Features.Core.Queries.GetTrustedDevices;

/// <summary>
/// Executes UC-AUTH-08 list. Returns the caller's own non-deleted trusted devices with the computed expiry
/// (<c>trusted_at + TrustedDevicePeriod</c>). Scoped to <c>ICurrentUser.UserId</c> + tenant — never another
/// user's devices (BR-AUTH-10). No fingerprint is exposed.
/// </summary>
internal sealed class GetTrustedDevicesQueryHandler
    : IRequestHandler<GetTrustedDevicesQuery, IReadOnlyList<TrustedDeviceResponse>>
{
    private readonly ICurrentUser _currentUser;
    private readonly ITrustedDeviceRepository _trustedDevices;

    public GetTrustedDevicesQueryHandler(ICurrentUser currentUser, ITrustedDeviceRepository trustedDevices)
    {
        _currentUser = currentUser;
        _trustedDevices = trustedDevices;
    }

    public async Task<IReadOnlyList<TrustedDeviceResponse>> Handle(GetTrustedDevicesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var tenantId = _currentUser.TenantId ?? throw new UnauthorizedException();

        var devices = await _trustedDevices.ListAsync(userId, tenantId, cancellationToken);

        return devices
            .Select(d => new TrustedDeviceResponse(
                d.TrustedDeviceId, d.DeviceName, d.Platform, d.TrustedAt, d.LastSeenAt, d.ExpiresAtUtc, d.Status))
            .ToList();
    }
}
