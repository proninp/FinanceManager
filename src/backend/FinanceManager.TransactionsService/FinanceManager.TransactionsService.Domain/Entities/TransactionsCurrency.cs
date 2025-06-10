using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

/// <summary>
/// Представляет валюту, используемую в транзакциях финансового менеджера
/// </summary>
/// <param name="name">Полное название валюты</param>
/// <param name="charCode">Буквенный код валюты (например, USD, RUB)</param>
/// <param name="numCode">Цифровой код валюты по стандарту ISO 4217</param>
/// <param name="sign">Символ валюты (например, $, €, ₽)</param>
/// <param name="emoji">Эмодзи для визуального представления валюты</param>
public class TransactionsCurrency(
    string name,
    string charCode,
    string numCode,
    string? sign = null,
    string? emoji = null) : IdentityModel
{
    /// <summary>
    /// Полное название валюты (например, "Российский рубль", "Доллар США")
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Буквенный код валюты (например, USD, EUR, RUB)
    /// </summary>
    public string CharCode { get; set; } = charCode;

    /// <summary>
    /// Цифровой код валюты по стандарту ISO 4217 (например, 840 для USD, 643 для RUB)
    /// </summary>
    public string NumCode { get; set; } = numCode;

    /// <summary>
    /// Символ валюты, используемый при отображении суммы (например, $, €, ₽)
    /// </summary>
    public string? Sign { get; set; } = sign;

    /// <summary>
    /// Эмодзи для визуального представления валюты (например, 💵, 💸)
    /// </summary>
    public string? Emoji { get; set; } = emoji;
}