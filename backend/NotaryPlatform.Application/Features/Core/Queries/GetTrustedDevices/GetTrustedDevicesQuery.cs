using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Behaviors;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Features.Core.Queries.GetTrustedDevices;

/// <summary>
/// UC-AUTH-08 — lists the signed-in user's own trusted devices (self-service management). A read ⇒
/// <see cref="IQuery{T}"/>. Authenticated <see cref="IAuthorizedRequest"/> with a null permission (any
/// signed-in user) — always scoped to <c>ICurrentUser</c>, so a caller only ever sees their own devices.
/// </summary>
public sealed record GetTrustedDevicesQuery
    : IQuery<IReadOnlyList<TrustedDeviceResponse>>, IAuthorizedRequest
{
    public string? RequiredPermission => null;   // any authenticated user
}
