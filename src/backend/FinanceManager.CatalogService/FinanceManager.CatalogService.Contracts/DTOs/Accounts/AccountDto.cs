using FinanceManager.CatalogService.Contracts.DTOs.AccountTypes;
using FinanceManager.CatalogService.Contracts.DTOs.Banks;
using FinanceManager.CatalogService.Contracts.DTOs.Currencies;
using FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;
using FinanceManager.CatalogService.Domain.Entities;

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
    bool IsDeleted = false,
    decimal? CreditLimit = null
);

/// <summary>
/// Методы-расширения для преобразования сущности Account в AccountDto
/// </summary>
public static class AccountDtoExtensions
{
    /// <summary>
    /// Преобразует сущность Account в DTO AccountDto
    /// </summary>
    /// <param name="account">Сущность банковского счета</param>
    /// <returns>Экземпляр AccountDto</returns>
    public static AccountDto ToDto(this Account account)
    {
        return new AccountDto(
            account.Id,
            account.RegistryHolder.ToDto(),
            account.AccountType.ToDto(),
            account.Currency.ToDto(),
            account.Bank.ToDto(),
            account.Name,
            account.IsIncludeInBalance,
            account.IsDefault,
            account.IsArchived,
            account.IsDeleted,
            account.CreditLimit
        );
    }
    
    /// <summary>
    /// Преобразует коллекцию Account в коллекцию AccountDto
    /// </summary>
    public static IEnumerable<AccountDto> ToDto(this IEnumerable<Account> accounts) =>
        accounts.Select(ToDto);
}