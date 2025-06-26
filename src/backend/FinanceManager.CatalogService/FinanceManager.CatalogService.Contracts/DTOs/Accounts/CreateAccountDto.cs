using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Contracts.DTOs.Accounts;

/// <summary>
/// DTO для создания банковского счета
/// </summary>
/// <param name="RegistryHolderId">Идентификатор владельца реестра</param>
/// <param name="AccountTypeId">Идентификатор типа счета</param>
/// <param name="CurrencyId">Идентификатор валюты счета</param>
/// <param name="BankId">Идентификатор банка</param>
/// <param name="Name">Название счета</param>
/// <param name="IsIncludeInBalance">Признак включения счета в общий баланс</param>
/// <param name="IsDefault">Признак счета по умолчанию</param>
/// <param name="CreditLimit">Кредитный лимит по счету</param>
public record CreateAccountDto(
    Guid RegistryHolderId,
    Guid AccountTypeId,
    Guid CurrencyId,
    Guid BankId,
    string Name,
    bool IsIncludeInBalance = false,
    bool IsDefault = false,
    decimal? CreditLimit = null
);

/// <summary>
/// Методы-расширения для преобразования CreateAccountDto в Account
/// </summary>
public static class CreateAccountDtoExtensions
{
    /// <summary>
    /// Преобразует DTO создания счета в сущность Account
    /// </summary>
    /// <param name="dto">DTO для создания банковского счета</param>
    /// <returns>Экземпляр Account</returns>
    public static Account ToAccount(this CreateAccountDto dto) =>
        new Account(
            dto.RegistryHolderId,
            dto.AccountTypeId,
            dto.CurrencyId,
            dto.BankId,
            dto.Name,
            dto.IsIncludeInBalance,
            dto.IsDefault,
            false,
            dto.CreditLimit
        );
}