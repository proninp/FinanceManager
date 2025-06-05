namespace FinanceManager.CatalogService.Contracts.DTOs.AccountTypes;

/// <summary>
/// DTO для обновления типа банковского счета
/// </summary>
/// <param name="Id">Идентификатор типа счета</param>
/// <param name="Code">Код типа счета</param>
/// <param name="Description">Описание типа счета</param>
public record UpdateAccountTypeDto(
    Guid Id,
    string? Code,
    string? Description
);