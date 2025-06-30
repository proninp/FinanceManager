using FinanceManager.TransactionsService.Domain.Entities;

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

/// <summary>
/// Методы-расширения для преобразования сущности TransactionsCurrency в TransactionCurrencyDto
/// </summary>
public static class TransactionCurrenciesExtensions
{
    /// <summary>
    /// Преобразует сущность TransactionsCurrency в DTO TransactionCurrencyDto
    /// </summary>
    /// <param name="currency">Сущность валюты</param>
    /// <returns>Экземпляр TransactionCurrencyDto</returns>
    public static TransactionCurrencyDto ToDto(this TransactionsCurrency currency)
    {
        return new TransactionCurrencyDto(
            currency.Id,
            currency.CharCode,
            currency.NumCode
        );
    }

    /// <summary>
    /// Преобразует коллекцию сущностей TransactionsCurrency в коллекцию DTO TransactionCurrencyDto
    /// </summary>
    /// <param name="currencies">Коллекция сущностей валют</param>
    /// <returns>Коллекция TransactionCurrencyDto</returns>
    public static ICollection<TransactionCurrencyDto> ToDto(this IEnumerable<TransactionsCurrency> currencies)
    {
        return currencies.Select(ToDto).ToList();
    }
}