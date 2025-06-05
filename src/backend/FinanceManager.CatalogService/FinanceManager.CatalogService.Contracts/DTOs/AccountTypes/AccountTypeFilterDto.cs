namespace FinanceManager.CatalogService.Contracts.DTOs.AccountTypes;

/// <summary>
/// DTO для фильтрации и пагинации типов счетов
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="Code">Код типа счета</param>
/// <param name="Description">Описание типа счета</param>
public record AccountTypeFilterDto(
    int ItemsPerPage,
    int Page,
    string? Code,
    string? Description
);