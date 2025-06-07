using FinanceManager.CatalogService.Contracts.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Abstractions;

namespace FinanceManager.CatalogService.Contracts.DTOs.Accounts;

/// <summary>
/// DTO для фильтрации и пагинации счетов
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="RegistryHolderId">Идентификатор владельца счета</param>
/// <param name="AccountTypeId">Идентификатор типа счета</param>
/// <param name="CurrencyId">Идентификатор валюты счета</param>
/// <param name="BankId">Идентификатор банка</param>
/// <param name="NameContains">Содержит название счета</param>
/// <param name="IsIncludeInBalance">Включать ли счет в общий баланс</param>
/// <param name="IsDefault">Является ли счет по умолчанию</param>
/// <param name="IsArchived">Архивирован ли счет</param>
/// <param name="CreditLimitFrom">Начальный диапазон лимита счета</param>
/// /// <param name="CreditLimitTo">Конечный диапазон лимита счета</param>
public record AccountFilterDto(
    int ItemsPerPage,
    int Page,
    Guid? RegistryHolderId = null,
    Guid? AccountTypeId = null,
    Guid? CurrencyId = null,
    Guid? BankId = null,
    string? NameContains = null,
    bool? IsIncludeInBalance = null,
    bool? IsDefault = null,
    bool? IsArchived = null,
    decimal? CreditLimitFrom = null,
    decimal? CreditLimitTo = null
) : BasePaginationDto(ItemsPerPage, Page);