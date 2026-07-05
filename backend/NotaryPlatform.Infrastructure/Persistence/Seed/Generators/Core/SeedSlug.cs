using System.Text;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>
/// Turns a display name into a short, uppercase, hyphenated code compatible
/// with the <c>TenantCode</c>/<c>OrganizationCode</c> value object pattern
/// (<c>^[A-Z0-9][A-Z0-9_-]{2,29}$</c>).
/// </summary>
public static class SeedSlug
{
    public static string From(string value, int maxLength = 30)
    {
        var builder = new StringBuilder();

        foreach (var ch in value.Trim().ToUpperInvariant())
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
            }
            else if (builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        var slug = builder.ToString().Trim('-');

        if (slug.Length > maxLength)
        {
            slug = slug[..maxLength].Trim('-');
        }

        return slug.Length >= 3 ? slug : slug.PadRight(3, 'X');
    }
}
