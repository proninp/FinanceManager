namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionCurrencies;

public record TransactionCurrencyDto(
    Guid Id,
    string CharCode,
    string NumCode);