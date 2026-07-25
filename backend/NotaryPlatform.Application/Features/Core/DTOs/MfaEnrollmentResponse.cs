namespace NotaryPlatform.Application.Features.Core.DTOs;

/// <summary>
/// Result of starting TOTP enrollment (UC-AUTH-06 Step A). The raw <see cref="Secret"/> and
/// <see cref="OtpauthUri"/> are returned exactly once (over HTTPS) for the client to render a QR code —
/// the secret is never returned again and is never logged. Only its encrypted reference is persisted.
/// </summary>
public sealed record MfaEnrollmentResponse(
    Guid MfaDeviceId,
    string Secret,
    string OtpauthUri);
