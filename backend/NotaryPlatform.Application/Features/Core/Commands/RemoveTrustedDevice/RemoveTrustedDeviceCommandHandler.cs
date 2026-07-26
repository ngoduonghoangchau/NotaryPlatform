using MediatR;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Shared.Exceptions;

namespace NotaryPlatform.Application.Features.Core.Commands.RemoveTrustedDevice;

/// <summary>
/// Executes UC-AUTH-08 remove. Soft-revokes the caller's own trusted device via the repository's ownership
/// guard: when no non-deleted device with that id belongs to (user, tenant), the revoke returns false and
/// this handler throws <see cref="NotFoundException"/> (404) — not 403 — so a caller cannot probe for the
/// existence of another user's device (anti-enumeration). Idempotent for an already-revoked device.
/// </summary>
internal sealed class RemoveTrustedDeviceCommandHandler : IRequestHandler<RemoveTrustedDeviceCommand>
{
    private readonly ICurrentUser _currentUser;
    private readonly ITrustedDeviceRepository _trustedDevices;
    private readonly IDateTime _clock;

    public RemoveTrustedDeviceCommandHandler(
        ICurrentUser currentUser,
        ITrustedDeviceRepository trustedDevices,
        IDateTime clock)
    {
        _currentUser = currentUser;
        _trustedDevices = trustedDevices;
        _clock = clock;
    }

    public async Task Handle(RemoveTrustedDeviceCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var tenantId = _currentUser.TenantId ?? throw new UnauthorizedException();

        var revoked = await _trustedDevices.RevokeAsync(
            request.TrustedDeviceId, userId, tenantId, _clock.UtcNow.UtcDateTime, cancellationToken);

        if (!revoked)
            throw new NotFoundException("TrustedDevice", request.TrustedDeviceId);   // 404 (ownership guard)

        // TransactionBehavior commits the soft-revoke after this returns.
    }
}
