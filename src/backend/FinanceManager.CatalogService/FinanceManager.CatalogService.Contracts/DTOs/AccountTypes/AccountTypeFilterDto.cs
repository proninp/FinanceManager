using FinanceManager.CatalogService.Contracts.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Abstractions;

namespace FinanceManager.CatalogService.Contracts.DTOs.AccountTypes;

/// <summary>
/// DTO для фильтрации и пагинации типов счетов
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="Code">Код типа счета</param>
/// <param name="DescriptionContains">Содержит описание типа счета</param>
public record AccountTypeFilterDto(
    int ItemsPerPage,
    int Page,
    string? Code = null,
    string? DescriptionContains = null
) : BasePaginationDto(ItemsPerPage, Page);