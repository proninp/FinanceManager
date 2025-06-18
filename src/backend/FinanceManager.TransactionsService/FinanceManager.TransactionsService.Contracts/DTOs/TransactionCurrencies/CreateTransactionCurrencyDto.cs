namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionCurrencies;

/// <summary>
/// DTO для создания валюты
/// </summary>
/// <param name="Name">Название валюты</param>
/// <param name="CharCode">Символьный код валюты</param>
/// <param name="NumCode">Числовой код валюты</param>
public record CreateTransactionCurrencyDto(
    string Name,
    string CharCode,
    string NumCode
);