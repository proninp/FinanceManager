using FinanceManager.TransactionsService.Contracts.DTOs.Abstractions;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionCurrencies;

/// <summary>
/// DTO для фильтрации и пагинации валют
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="CharCode">Символьный код валюты</param>
/// <param name="NumCode">Числовой код валюты</param>
public record TransactionCurrencyFilterDto(
    int ItemsPerPage,
    int Page,
    string? CharCode = null,
    string? NumCode = null
) : BasePaginationDto(ItemsPerPage, Page);