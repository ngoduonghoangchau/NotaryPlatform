namespace NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

/// <summary>
/// Data volume per <see cref="SeedProfile"/>. Seeders read this instead of
/// hard-coding counts, so tuning volume never requires touching seeder logic.
/// </summary>
public sealed record SeedVolumePlan(
    int TenantCount,
    int OrganizationsPerTenant,
    int BranchesPerOrganization,
    int RegionsPerOrganization,
    int TeamsPerBranch,
    int UsersPerTenant,
    int RefreshTokensPerActiveUser)
{
    public static SeedVolumePlan For(SeedProfile profile) => profile switch
    {
        SeedProfile.Development => new SeedVolumePlan(
            TenantCount: 1,
            OrganizationsPerTenant: 1,
            BranchesPerOrganization: 2,
            RegionsPerOrganization: 1,
            TeamsPerBranch: 2,
            UsersPerTenant: 8,
            RefreshTokensPerActiveUser: 1),

        SeedProfile.Demo => new SeedVolumePlan(
            TenantCount: 4,
            OrganizationsPerTenant: 2,
            BranchesPerOrganization: 2,
            RegionsPerOrganization: 2,
            TeamsPerBranch: 2,
            UsersPerTenant: 15,
            RefreshTokensPerActiveUser: 2),

        SeedProfile.Staging => new SeedVolumePlan(
            TenantCount: 6,
            OrganizationsPerTenant: 2,
            BranchesPerOrganization: 3,
            RegionsPerOrganization: 2,
            TeamsPerBranch: 3,
            UsersPerTenant: 25,
            RefreshTokensPerActiveUser: 2),

        // Production never receives fake tenants/users — only tenant-independent
        // reference catalogs (permissions) are bootstrapped.
        SeedProfile.ProductionBootstrap => new SeedVolumePlan(0, 0, 0, 0, 0, 0, 0),

        _ => throw new ArgumentOutOfRangeException(nameof(profile), profile, "Unknown seed profile.")
    };
}
