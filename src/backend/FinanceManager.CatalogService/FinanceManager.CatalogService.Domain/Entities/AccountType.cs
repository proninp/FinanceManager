using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

/// <summary>
/// Представляет тип финансового счета
/// </summary>
/// <param name="code">Код типа счета</param>
/// <param name="description">Описание типа счета</param>
public class AccountType(string code, string description) : SoftDeletableEntity
{
    /// <summary>
    /// Уникальный код типа счета
    /// </summary>
    public string Code { get; set; } =  code;

    /// <summary>
    /// Описание типа счета
    /// </summary>
    public string Description { get; set; } = description;
}