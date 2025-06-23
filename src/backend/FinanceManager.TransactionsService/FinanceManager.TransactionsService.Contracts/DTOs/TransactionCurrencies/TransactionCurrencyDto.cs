namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionCurrencies;

/// <summary>
/// DTO для представления валюты транзакции
/// </summary>
/// <param name="Id">Уникальный идентификатор валюты</param>
/// <param name="CharCode">Буквенный код валюты (например, USD, RUB)</param>
/// <param name="NumCode">Цифровой код валюты (например, 840 для USD, 643 для RUB)</param>
public record TransactionCurrencyDto(
    Guid Id,
    string CharCode,
    string NumCode);