using System.Text.RegularExpressions;
using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Core.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex Pattern = new("^\\+?[0-9()\\-\\s]{7,25}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string value)
    {
        var normalized = Normalize(value);

        if (!Pattern.IsMatch(normalized))
        {
            throw new BusinessRuleValidationException("Invalid phone number format.");
        }

        return new PhoneNumber(normalized);
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Phone number is required.");
        }

        return value.Trim();
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
