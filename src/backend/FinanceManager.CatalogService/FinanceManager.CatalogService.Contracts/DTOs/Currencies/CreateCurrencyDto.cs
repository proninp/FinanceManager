using FinanceManager.CatalogService.Domain.Entities;

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
    string? Sign = null,
    string? Emoji = null
);

/// <summary>
/// Методы-расширения для преобразования CreateCurrencyDto в Currency
/// </summary>
public static class CreateCurrencyDtoExtensions
{
    /// <summary>
    /// Преобразует DTO создания валюты в сущность Currency
    /// </summary>
    /// <param name="dto">DTO для создания валюты</param>
    /// <returns>Экземпляр Currency</returns>
    public static Currency ToCurrency(this CreateCurrencyDto dto) =>
        new Currency(dto.Name, dto.CharCode, dto.NumCode, dto.Sign, dto.Emoji);
}