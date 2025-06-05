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
    bool IsIncludeInBalance,
    bool IsDefault,
    decimal? CreditLimit
);