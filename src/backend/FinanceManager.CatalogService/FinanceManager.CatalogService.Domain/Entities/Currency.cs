using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

/// <summary>
/// Представляет справочник валют
/// </summary>
/// <param name="name">Название валюты</param>
/// <param name="charCode">Символьный код валюты</param>
/// <param name="numCode">Числовой код валюты</param>
/// <param name="sign">Символ валюты</param>
/// <param name="emoji">Эмодзи валюты</param>
public class Currency(string name, string charCode, string numCode, string? sign, string? emoji)
    : SoftDeletableEntity
{
    /// <summary>
    /// Полное название валюты
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Символьный код валюты (например, USD, EUR)
    /// </summary>
    public string CharCode { get; set; } = charCode;

    /// <summary>
    /// Числовой код валюты согласно ISO 4217
    /// </summary>
    public string NumCode { get; set; } = numCode;

    /// <summary>
    /// Символ валюты (например, $, €)
    /// </summary>
    public string? Sign { get; set; } = sign;

    /// <summary>
    /// Эмодзи для представления валюты
    /// </summary>
    public string? Emoji { get; set; } = emoji;
}