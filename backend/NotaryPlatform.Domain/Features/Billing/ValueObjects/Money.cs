using System.Collections.Generic;
using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Billing.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new BusinessRuleValidationException("Amount cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new BusinessRuleValidationException("Currency is required.");
        }

        var normalizedCurrency = currency.Trim().ToUpperInvariant();

        if (normalizedCurrency.Length != 3)
        {
            throw new BusinessRuleValidationException("Currency must be a 3-letter ISO code.");
        }

        return new Money(decimal.Round(amount, 2), normalizedCurrency);
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return Create(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        var result = Amount - other.Amount;
        if (result < 0)
        {
            throw new BusinessRuleValidationException("Money value cannot become negative.");
        }

        return Create(result, Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (other is null)
        {
            throw new BusinessRuleValidationException("Money value is required.");
        }

        if (!string.Equals(Currency, other.Currency, System.StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessRuleValidationException("Cannot operate on different currencies.");
        }
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}
