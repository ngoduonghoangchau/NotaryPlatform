using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Core.ValueObjects;

public sealed class UserName : ValueObject
{
    public string Value { get; }

    private UserName(string value)
    {
        Value = value;
    }

    public static UserName Create(string value)
    {
        var normalized = Normalize(value);

        if (normalized.Length < 2 || normalized.Length > 150)
        {
            throw new BusinessRuleValidationException("User name must be between 2 and 150 characters.");
        }

        return new UserName(normalized);
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("User name is required.");
        }

        return value.Trim();
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
