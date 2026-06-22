using System.Collections.Generic;
using System.Text.RegularExpressions;
using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Operations.ValueObjects;

public sealed class DispatchCode : ValueObject
{
    private static readonly Regex Pattern = new("^[A-Z0-9][A-Z0-9_-]{2,49}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private DispatchCode(string value)
    {
        Value = value;
    }

    public static DispatchCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Dispatch code is required.");
        }

        var normalized = value.Trim().ToUpperInvariant();

        if (!Pattern.IsMatch(normalized))
        {
            throw new BusinessRuleValidationException(
                "Dispatch code must be 3-50 characters and contain only upper-case letters, digits, underscore or hyphen.");
        }

        return new DispatchCode(normalized);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
