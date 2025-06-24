using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

/// <summary>
/// Представляет валюту, используемую в транзакциях финансового менеджера
/// </summary>
/// <param name="charCode">Буквенный код валюты (например, USD, RUB)</param>
/// <param name="numCode">Цифровой код валюты по стандарту ISO 4217</param>
public class TransactionsCurrency(
    string charCode,
    string numCode) : IdentityModel
{
    /// <summary>
    /// Буквенный код валюты (например, USD, EUR, RUB)
    /// </summary>
    public string CharCode { get; set; } = charCode;

    /// <summary>
    /// Цифровой код валюты по стандарту ISO 4217 (например, 840 для USD, 643 для RUB)
    /// </summary>
    public string NumCode { get; set; } = numCode;

}

