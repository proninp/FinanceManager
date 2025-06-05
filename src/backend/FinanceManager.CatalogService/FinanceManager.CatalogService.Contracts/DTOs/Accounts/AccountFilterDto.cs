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
/// <param name="Name">Название счета</param>
/// <param name="IsIncludeInBalance">Включать ли счет в общий баланс</param>
/// <param name="IsDefault">Является ли счет по умолчанию</param>
/// <param name="IsArchived">Архивирован ли счет</param>
/// <param name="CreditLimit">Кредитный лимит счета</param>
public record AccountFilterDto(
    int ItemsPerPage,
    int Page,
    Guid? RegistryHolderId,
    Guid? AccountTypeId,
    Guid? CurrencyId,
    Guid? BankId,
    string? Name,
    bool? IsIncludeInBalance,
    bool? IsDefault,
    bool? IsArchived,
    decimal? CreditLimit
);