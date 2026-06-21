using System.Net.Mail;
using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Core.ValueObjects;

public sealed class EmailAddress : ValueObject
{
    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static EmailAddress Create(string value)
    {
        var normalized = Normalize(value);

        try
        {
            _ = new MailAddress(normalized);
        }
        catch
        {
            throw new BusinessRuleValidationException("Invalid email address format.");
        }

        return new EmailAddress(normalized);
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Email address is required.");
        }

        return value.Trim().ToLowerInvariant();
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
