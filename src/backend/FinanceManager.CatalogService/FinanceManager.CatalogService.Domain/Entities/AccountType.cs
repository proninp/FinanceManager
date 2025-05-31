using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

/// <summary>
/// Представляет тип финансового счета
/// </summary>
/// <param name="code">Код типа счета</param>
/// <param name="description">Описание типа счета</param>
/// <param name="isDeleted">Удален ли тип счета</param>
public class AccountType(string code, string description, bool isDeleted = false) : IdentityModel
{
    /// <summary>
    /// Уникальный код типа счета
    /// </summary>
    public string Code { get; set; } =  code;

    /// <summary>
    /// Описание типа счета
    /// </summary>
    public string Description { get; set; } = description;

    /// <summary>
    /// Флаг мягкого удаления типа счета
    /// </summary>
    public bool IsDeleted { get; set; } = isDeleted;
}