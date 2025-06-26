using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Contracts.DTOs.Currencies;

/// <summary>
/// DTO для валюты
/// </summary>
/// <param name="Id">Идентификатор валюты</param>
/// <param name="Name">Название валюты</param>
/// <param name="CharCode">Символьный код валюты (например, USD, EUR)</param>
/// <param name="NumCode">Числовой код валюты согласно ISO 4217</param>
/// <param name="Sign">Символ валюты (например, $, €)</param>
/// <param name="Emoji">Эмодзи для представления валюты</param>
public record CurrencyDto(
    Guid Id,
    string Name,
    string CharCode,
    string NumCode,
    string? Sign = null,
    string? Emoji = null
);

/// <summary>
/// Методы-расширения для преобразования сущности Currency в CurrencyDto
/// </summary>
public static class CurrencyDtoExtensions
{
    /// <summary>
    /// Преобразует сущность Currency в DTO CurrencyDto
    /// </summary>
    /// <param name="currency">Сущность валюты</param>
    /// <returns>Экземпляр CurrencyDto</returns>
    public static CurrencyDto ToDto(this Currency currency) =>
        new CurrencyDto(
            currency.Id,
            currency.Name,
            currency.CharCode,
            currency.NumCode,
            currency.Sign,
            currency.Emoji);

    /// <summary>
    /// Преобразует коллекцию Currency в коллекцию CurrencyDto
    /// </summary>
    /// <param name="currencies">Коллекция сущностей валют</param>
    /// <returns>Коллекция CurrencyDto</returns>
    public static ICollection<CurrencyDto> ToDto(this IEnumerable<Currency> currencies)
    {
        var dtos = currencies.Select(ToDto);
        return dtos as ICollection<CurrencyDto> ?? dtos.ToList();
    }
}