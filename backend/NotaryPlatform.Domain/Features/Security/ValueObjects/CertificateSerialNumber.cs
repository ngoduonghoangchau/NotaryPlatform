using System.Collections.Generic;
using System.Text.RegularExpressions;
using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Security.ValueObjects;

public sealed class CertificateSerialNumber : ValueObject
{
    private static readonly Regex Pattern = new("^[A-Z0-9][A-Z0-9_-]{2,79}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private CertificateSerialNumber(string value)
    {
        Value = value;
    }

    public static CertificateSerialNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Certificate serial number is required.");
        }

        var normalized = value.Trim().ToUpperInvariant();

        if (!Pattern.IsMatch(normalized))
        {
            throw new BusinessRuleValidationException(
                "Certificate serial number must be 3-80 characters and contain only upper-case letters, digits, underscore or hyphen.");
        }

        return new CertificateSerialNumber(normalized);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
