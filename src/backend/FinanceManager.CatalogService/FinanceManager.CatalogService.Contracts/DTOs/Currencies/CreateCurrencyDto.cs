namespace FinanceManager.CatalogService.Contracts.DTOs.Currencies;

/// <summary>
/// DTO для создания валюты
/// </summary>
/// <param name="Name">Название валюты</param>
/// <param name="CharCode">Символьный код валюты</param>
/// <param name="NumCode">Числовой код валюты</param>
/// <param name="Sign">Символ валюты</param>
/// <param name="Emoji">Эмодзи валюты</param>
public record CreateCurrencyDto(
    string Name,
    string CharCode,
    string NumCode,
    string? Sign,
    string? Emoji
);