namespace NotaryPlatform.Application.Common.Models.Auth;

/// <summary>
/// Data bundle assembled by the login / token-refresh use case and passed to
/// IJwtTokenService.CreateAccessToken to embed claims in the JWT.
/// Lives in Application because both the use case (Application) and the token
/// service implementation (Infrastructure) need to reference it.
/// </summary>
public sealed record JwtTokenClaims
{
    public required Guid UserId { get; init; }
    public required Guid TenantId { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public Guid? BranchId { get; init; }
    public Guid? RegionId { get; init; }
    public required IReadOnlyList<string> Roles { get; init; }
    public required IReadOnlyList<string> Permissions { get; init; }
}
