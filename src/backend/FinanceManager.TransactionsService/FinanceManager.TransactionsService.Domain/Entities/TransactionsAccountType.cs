using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

/// <summary>
/// Представляет тип банковского или финансового счёта
/// </summary>
/// <param name="code">Уникальный код типа счёта</param>
/// <param name="description">Описание типа счёта</param>
public class TransactionsAccountType(string code, string description):IdentityModel
{
    /// <summary>
    /// Уникальный код типа счёта, используемый для идентификации и логики приложения
    /// </summary>
    public string Code { get; set; } = code;
    
    /// <summary>
    /// Описание типа счёта, предназначенное для отображения пользователю
    /// </summary>
    public string Description { get; set; } = description;
}