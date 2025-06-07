using FinanceManager.CatalogService.Contracts.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Abstractions;

namespace FinanceManager.CatalogService.Contracts.DTOs.Categories;

/// <summary>
/// DTO для фильтрации и пагинации категорий
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="RegistryHolderId">Идентификатор владельца категории</param>
/// <param name="NameContains">Содержит название категории</param>
/// <param name="Income">Фильтр по доходным категориям</param>
/// <param name="Expense">Фильтр по расходным категориям</param>
/// <param name="Emoji">Эмодзи категории</param>
/// <param name="HasIcon">Указана ли иконка категории</param>
/// <param name="HasTransactions">Существуют ли зарегистрированные транзакции по данной категории</param>
/// <param name="ParentId">Идентификатор родительской категории</param>
public record CategoryFilterDto(
    int ItemsPerPage,
    int Page,
    Guid? RegistryHolderId = null,
    string? NameContains = null,
    bool? Income = null,
    bool? Expense = null,
    string? Emoji = null,
    bool? HasIcon = null,
    bool? HasTransactions = null,
    Guid? ParentId = null
) : BasePaginationDto(ItemsPerPage, Page);