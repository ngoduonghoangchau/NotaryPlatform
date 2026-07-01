namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// Claim type constants used when writing (JwtTokenService) and reading
/// (CurrentUserService) JWT tokens.
/// Kept internal to Infrastructure — Application only sees ICurrentUser
/// and IJwtTokenService contracts, never raw claim names.
/// </summary>
internal static class JwtClaimTypes
{
    internal const string UserId = "uid";
    internal const string TenantId = "tid";
    internal const string BranchId = "bid";
    internal const string RegionId = "rid";
    internal const string Permissions = "permissions";
}
