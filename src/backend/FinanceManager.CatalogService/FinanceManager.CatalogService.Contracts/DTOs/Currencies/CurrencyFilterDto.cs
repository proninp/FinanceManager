namespace FinanceManager.CatalogService.Contracts.DTOs.Currencies;

/// <summary>
/// DTO для фильтрации и пагинации валют
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="Name">Название валюты</param>
/// <param name="CharCode">Символьный код валюты</param>
/// <param name="NumCode">Числовой код валюты</param>
/// <param name="Sign">Символ валюты</param>
/// <param name="Emoji">Эмодзи валюты</param>
/// <param name="HasExchangeRates">Есть ли у валюты курсы</param>
public record CurrencyFilterDto(
    int ItemsPerPage,
    int Page,
    string? Name,
    string? CharCode,
    string? NumCode,
    string? Sign,
    string? Emoji,
    bool? HasExchangeRates
);