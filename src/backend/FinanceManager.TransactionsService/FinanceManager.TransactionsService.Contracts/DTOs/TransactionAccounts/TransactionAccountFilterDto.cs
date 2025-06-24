using FinanceManager.TransactionsService.Contracts.DTOs.Abstractions;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;
    
/// <summary>
/// DTO для фильтрации и пагинации счетов
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="TransactionHolderId">Идентификатор владельца счета</param>
/// <param name="AccountTypeId">Идентификатор типа счета</param>
/// <param name="CurrencyId">Идентификатор валюты счета</param>
/// <param name="CreditLimitFrom">Начальный диапазон лимита счета</param>
/// /// <param name="CreditLimitTo">Конечный диапазон лимита счета</param>
public record TransactionAccountFilterDto(
    int ItemsPerPage,
    int Page,
    Guid? TransactionHolderId = null,
    Guid? AccountTypeId = null,
    Guid? CurrencyId = null,
    decimal? CreditLimitFrom = null,
    decimal? CreditLimitTo = null
) : BasePaginationDto(ItemsPerPage, Page);