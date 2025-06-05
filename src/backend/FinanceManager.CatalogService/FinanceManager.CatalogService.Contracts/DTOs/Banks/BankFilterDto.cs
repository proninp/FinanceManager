using FinanceManager.CatalogService.Contracts.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Abstractions;

namespace FinanceManager.CatalogService.Contracts.DTOs.Banks;

/// <summary>
/// DTO для фильтрации и пагинации банков
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="CountryId">Идентификатор страны</param>
/// <param name="NameContains">Содержит название банка</param>
public record BankFilterDto(
    int ItemsPerPage = PaginationDefaults.DefaultItemsPerPage,
    int Page = PaginationDefaults.DefaultPage,
    Guid? CountryId = null,
    string? NameContains = null
) : BasePaginationDto(ItemsPerPage, Page);