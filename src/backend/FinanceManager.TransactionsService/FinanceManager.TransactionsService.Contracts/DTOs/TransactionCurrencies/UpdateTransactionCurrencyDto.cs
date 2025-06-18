namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionCurrencies;

/// <summary>
/// DTO для обновления валюты
/// </summary>
/// <param name="Id">Идентификатор валюты</param>
/// <param name="Name">Название валюты</param>
/// <param name="CharCode">Символьный код валюты</param>
/// <param name="NumCode">Числовой код валюты</param>

public record UpdateTransactionCurrencyDto(
    Guid Id,
    string? Name = null,
    string? CharCode = null,
    string? NumCode = null
);