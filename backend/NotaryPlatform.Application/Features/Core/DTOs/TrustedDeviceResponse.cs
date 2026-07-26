namespace NotaryPlatform.Application.Features.Core.DTOs;

/// <summary>
/// One of the caller's trusted devices (UC-AUTH-08), for the self-service management list.
/// <see cref="ExpiresAtUtc"/> is computed (<c>trusted_at + TrustedDevicePeriod</c>) — there is no stored
/// expiry column. No fingerprint is ever returned to the client.
/// </summary>
public sealed record TrustedDeviceResponse(
    Guid TrustedDeviceId,
    string? DeviceName,
    string? Platform,
    DateTime? TrustedAt,
    DateTime? LastSeenAt,
    DateTime? ExpiresAtUtc,
    string Status);
