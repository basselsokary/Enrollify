using Domain.Common;
using Domain.Common.Shared;

namespace Domain.ValueObjects;

public sealed class Money : ValueObject 
{
    public const int Precision = 18;
    public const int Scale = 2;
    public const int MaxCurrencyLength = 3;

    public decimal Amount { get; }
    public string Currency { get; } = null!;
    
    public static Money Zero => new(0, "EGP");

    private Money() { }
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency = "EGP")
    {
        if (amount < 0)
            return MoneyErrors.NegativeAmount;

        if (string.IsNullOrWhiteSpace(currency))
            return MoneyErrors.CurrencyRequired;

        if (currency.Length != 3)
            return MoneyErrors.InvalidCurrencyFormat;

        return new Money(amount, currency.ToUpperInvariant());
    }

    public Result<Money> Add(Money other)
    {
        if (Currency != other.Currency)
            return MoneyErrors.CurrencyMismatch;

        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// This method does not handle currency conversion. It assumes both Money instances are in the same currency.
    /// </summary>
    public Money Add(decimal amount)
    {
        return new Money(Amount + amount, Currency);
    }

    public Result<Money> Subtract(Money other)
    {
        if (Currency != other.Currency)
            return MoneyErrors.CurrencyMismatch;

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        return new Money(Amount * factor, Currency);
    }

    public Result<Money> Divide(decimal divisor)
    {
        if (divisor == 0)
            return MoneyErrors.DivisionByZero;

        return new Money(Amount / divisor, Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency} {Amount:N2}";

    public static bool operator >(Money a, Money b) => a.Amount > b.Amount;
    public static bool operator <(Money a, Money b) => b > a;

    public static bool operator >=(Money a, Money b) => a.Amount >= b.Amount;
    public static bool operator <=(Money a, Money b) => b <= a;
}


public static class MoneyErrors
{
    public static Error NegativeAmount =>
        Error.Validation("MONEY_NEGATIVE_AMOUNT", "Amount cannot be negative.");

    public static Error CurrencyRequired =>
        Error.Validation("MONEY_CURRENCY_REQUIRED", "Currency is required.");

    public static Error InvalidCurrencyFormat =>
        Error.Validation("MONEY_INVALID_CURRENCY_FORMAT", "Currency must be a 3-letter ISO code.");

    public static Error CurrencyMismatch =>
        Error.Validation("MONEY_CURRENCY_MISMATCH", "Cannot operate on different currencies.");

    public static Error DivisionByZero =>
        Error.Validation("MONEY_DIVISION_BY_ZERO", "Cannot divide by zero.");
}