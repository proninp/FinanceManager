using FinanceManager.TransactionsService.Contracts.DTOs.Abstractions;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionsCategories;

/// <summary>
/// DTO для фильтрации и пагинации категорий
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="TransactionHolderId">Идентификатор владельца категории</param>
/// <param name="Income">Фильтр по доходным категориям</param>
/// <param name="Expense">Фильтр по расходным категориям</param>

public record TransactionCategoryFilterDto(
    int ItemsPerPage,
    int Page,
    Guid? TransactionHolderId = null,
    bool? Income = null,
    bool? Expense = null
) : BasePaginationDto(ItemsPerPage, Page);