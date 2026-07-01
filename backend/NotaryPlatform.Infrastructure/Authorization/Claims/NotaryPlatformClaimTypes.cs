namespace NotaryPlatform.Infrastructure.Authorization.Claims;

/// <summary>
/// Claim type constants embedded in JWTs by <c>JwtTokenService</c>
/// and consumed by authorization handlers, requirements, and evaluators.
/// Values must stay in sync with the private <c>JwtClaimTypes</c> constants
/// used internally by <c>JwtTokenService</c> and <c>CurrentUserService</c>.
/// </summary>
public static class NotaryPlatformClaimTypes
{
    /// <summary>User identity (<c>uid</c>).</summary>
    public const string UserId = "uid";

    /// <summary>Tenant the user is acting within (<c>tid</c>).</summary>
    public const string TenantId = "tid";

    /// <summary>Branch the user is assigned to (<c>bid</c>). Absent for users without a branch.</summary>
    public const string BranchId = "bid";

    /// <summary>Region the user is assigned to (<c>rid</c>). Absent for users without a region.</summary>
    public const string RegionId = "rid";

    /// <summary>
    /// Permission code claim (<c>permissions</c>).
    /// A user with N permissions has N separate claims of this type —
    /// one per code — so <c>ClaimsPrincipal.FindAll</c> must be used, not <c>FindFirst</c>.
    /// </summary>
    public const string Permissions = "permissions";
}
