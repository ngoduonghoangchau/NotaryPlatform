using System.Collections.Generic;
using System.Text.RegularExpressions;
using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Security.ValueObjects;

public sealed class DeviceFingerprint : ValueObject
{
    private static readonly Regex Pattern = new("^[A-Za-z0-9._:-]{8,200}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private DeviceFingerprint(string value)
    {
        Value = value;
    }

    public static DeviceFingerprint Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Device fingerprint is required.");
        }

        var normalized = value.Trim();

        if (!Pattern.IsMatch(normalized))
        {
            throw new BusinessRuleValidationException(
                "Device fingerprint contains invalid characters or length.");
        }

        return new DeviceFingerprint(normalized);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
