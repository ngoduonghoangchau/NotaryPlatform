using System.Collections.Generic;
using System.Text.RegularExpressions;
using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Communication.ValueObjects;

public sealed class MessageCode : ValueObject
{
    private static readonly Regex Pattern = new("^[A-Z0-9][A-Z0-9_-]{2,49}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private MessageCode(string value)
    {
        Value = value;
    }

    public static MessageCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Message code is required.");
        }

        var normalized = value.Trim().ToUpperInvariant();

        if (!Pattern.IsMatch(normalized))
        {
            throw new BusinessRuleValidationException(
                "Message code must be 3-50 characters and contain only upper-case letters, digits, underscore or hyphen.");
        }

        return new MessageCode(normalized);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
