using FinanceManager.TransactionsService.Contracts.DTOs.Abstractions;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionsCategories;

namespace FinanceManager.TransactionsService.Contracts.DTOs.Transactions;

/// <summary>
/// DTO для фильтрации и пагинации списка транзакций
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="DateFrom">Дата начала курса</param>
/// <param name="DateTo">Дата конца курса</param>
/// <param name="AccountId">Фильтр по идентификатору счёта, через который прошла транзакция</param>
/// <param name="CategoryId">Фильтр по идентификатору категории транзакции</param>
/// <param name="AmountFrom">Минимальная сумма транзакции включительно</param>
/// <param name="AmountTo">Максимальная сумма транзакции включительно</param>
/// <param name="DescriptionContains">Фильтр по содержанию текста в описании транзакции</param>
public record TransactionFilterDto(
    int ItemsPerPage,
    int Page,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    Guid? AccountId = null,
    Guid? CategoryId = null,
    decimal? AmountFrom = null,
    decimal? AmountTo = null,
    string? DescriptionContains = null
) : BasePaginationDto(ItemsPerPage, Page);

