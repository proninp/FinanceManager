using FinanceManager.CatalogService.Contracts.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Abstractions;

namespace FinanceManager.CatalogService.Contracts.DTOs.Countries;

/// <summary>
/// DTO для фильтрации и пагинации стран
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="NameContains">Содержит название страны</param>
public record CountryFilterDto(
    int ItemsPerPage,
    int Page,
    string? NameContains = null
) : BasePaginationDto(ItemsPerPage, Page);