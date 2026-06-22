using System.Collections.Generic;
using System.Text.RegularExpressions;
using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Operations.ValueObjects;

public sealed class JobCode : ValueObject
{
    private static readonly Regex Pattern = new("^[A-Z0-9][A-Z0-9_-]{2,49}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private JobCode(string value)
    {
        Value = value;
    }

    public static JobCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Job code is required.");
        }

        var normalized = value.Trim().ToUpperInvariant();

        if (!Pattern.IsMatch(normalized))
        {
            throw new BusinessRuleValidationException(
                "Job code must be 3-50 characters and contain only upper-case letters, digits, underscore or hyphen.");
        }

        return new JobCode(normalized);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
