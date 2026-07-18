namespace Application.Contracts.Common;

public record MoneyDto(
    decimal Amount,
    string Currency,
    string FormattedAmount);
