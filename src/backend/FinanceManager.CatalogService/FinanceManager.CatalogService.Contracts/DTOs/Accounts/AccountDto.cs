using FinanceManager.CatalogService.Contracts.DTOs.AccountTypes;
using FinanceManager.CatalogService.Contracts.DTOs.Banks;
using FinanceManager.CatalogService.Contracts.DTOs.Currencies;
using FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;

namespace FinanceManager.CatalogService.Contracts.DTOs.Accounts;

/// <summary>
/// DTO для банковского счета пользователя
/// </summary>
/// <param name="Id">Идентификатор счета</param>
/// <param name="RegistryHolder">Владелец счета</param>
/// <param name="AccountType">Тип счета</param>
/// <param name="Currency">Валюта счета</param>
/// <param name="Bank">Банк, в котором открыт счет</param>
/// <param name="Name">Название счета</param>
/// <param name="IsIncludeInBalance">Включать ли счет в общий баланс</param>
/// <param name="IsDefault">Является ли счет по умолчанию</param>
/// <param name="IsArchived">Архивирован ли счет</param>
/// <param name="CreditLimit">Кредитный лимит счета</param>
public record AccountDto(
    Guid Id,
    RegistryHolderDto RegistryHolder,
    AccountTypeDto AccountType,
    CurrencyDto Currency,
    BankDto Bank,
    string Name,
    bool IsIncludeInBalance,
    bool IsDefault,
    bool IsArchived,
    decimal? CreditLimit = null
);