namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>
/// Curated catalogs describing a realistic US notary-services enterprise.
/// Kept separate from Bogus-driven randomization so that company names,
/// metro areas, org divisions, and job titles always read as plausible
/// even though person names, phone numbers, and timestamps are fake.
/// </summary>
public static class UsEnterpriseReferenceData
{
    public sealed record StateProfile(string StateCode, string StateName, string[] Cities, string TimeZoneId);

    public static readonly string[] TenantNames =
    [
        "Pacific Notary Holdings",
        "Premier Escrow Services",
        "National Signing Network",
        "Liberty Title Group",
        "American Closing Solutions",
        "EverTrust Notary Services",
        "Prime Settlement Group",
        "West Coast Signing Services",
        "NorthStar Escrow Holdings",
        "United Document Services"
    ];

    public static readonly string[] LegalSuffixes = ["LLC", "Inc.", "Group LLC", "Holdings, Inc."];

    public static readonly string[] OrganizationDivisions =
    [
        "California Operations",
        "Texas Operations",
        "Florida Operations",
        "East Coast Operations",
        "West Coast Operations"
    ];

    public static readonly StateProfile[] States =
    [
        new("CA", "California", ["Los Angeles", "San Diego", "Sacramento", "San Jose", "San Francisco"], "America/Los_Angeles"),
        new("TX", "Texas", ["Dallas", "Houston", "Austin", "Fort Worth"], "America/Chicago"),
        new("FL", "Florida", ["Miami", "Orlando", "Tampa"], "America/New_York"),
        new("WA", "Washington", ["Seattle", "Bellevue"], "America/Los_Angeles"),
        new("NY", "New York", ["New York City", "Buffalo"], "America/New_York")
    ];

    public static readonly string[] RegionNames =
    [
        "Northern California",
        "Southern California",
        "Bay Area",
        "Central Texas",
        "North Texas",
        "South Florida",
        "Pacific Northwest",
        "Northeast Region"
    ];

    public static readonly string[] TeamNames =
    [
        "Mobile Signing Team",
        "Mortgage Signing Team",
        "Real Estate Closing Team",
        "Escrow Operations Team",
        "Remote Online Notary Team",
        "Compliance Review Team",
        "Customer Success Team",
        "Document Processing Team",
        "Operations Team",
        "Quality Assurance Team"
    ];

    public static readonly string[] JobTitles =
    [
        "Chief Executive Officer",
        "Chief Operations Officer",
        "Regional Director",
        "Branch Manager",
        "Operations Manager",
        "Notary Manager",
        "Escrow Officer",
        "Closing Specialist",
        "Compliance Officer",
        "Customer Support Specialist",
        "Billing Specialist",
        "System Administrator"
    ];

    public static readonly string[] Browsers = ["Chrome", "Edge", "Safari", "Firefox"];

    public static readonly string[] Devices = ["Windows Desktop", "MacBook Pro", "Surface Pro", "iPhone", "Android Phone"];

    /// <summary>Best-effort state lookup for a curated region name (e.g. "South Florida" → FL).</summary>
    public static StateProfile StateForRegionName(string regionName) => regionName switch
    {
        "Northern California" or "Southern California" or "Bay Area" => States[0],
        "Central Texas" or "North Texas" => States[1],
        "South Florida" => States[2],
        "Pacific Northwest" => States[3],
        "Northeast Region" => States[4],
        _ => States[0]
    };
}
