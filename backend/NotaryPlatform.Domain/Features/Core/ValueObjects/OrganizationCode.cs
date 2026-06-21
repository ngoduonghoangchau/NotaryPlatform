using System.Text.RegularExpressions;
using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Core.ValueObjects;

public sealed class OrganizationCode : ValueObject
{
    private static readonly Regex Pattern = new("^[A-Z0-9][A-Z0-9_-]{2,29}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private OrganizationCode(string value)
    {
        Value = value;
    }

    public static OrganizationCode Create(string value)
    {
        var normalized = Normalize(value);

        if (!Pattern.IsMatch(normalized))
        {
            throw new BusinessRuleValidationException(
                "Organization code must be 3-30 characters, use upper-case letters, digits, underscore or hyphen.");
        }

        return new OrganizationCode(normalized);
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Organization code is required.");
        }

        return value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
