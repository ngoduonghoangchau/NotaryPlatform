using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Core.ValueObjects;

public sealed class TimeZoneId : ValueObject
{
    public string Value { get; }

    private TimeZoneId(string value)
    {
        Value = value;
    }

    public static TimeZoneId Create(string value)
    {
        var normalized = Normalize(value);

        try
        {
            _ = TimeZoneInfo.FindSystemTimeZoneById(normalized);
        }
        catch
        {
            throw new BusinessRuleValidationException($"Invalid time zone id: {normalized}");
        }

        return new TimeZoneId(normalized);
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Time zone id is required.");
        }

        return value.Trim();
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
