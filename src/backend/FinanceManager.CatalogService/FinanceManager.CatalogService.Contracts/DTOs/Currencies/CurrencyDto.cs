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