using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

/// <summary>
/// Банковский счет пользователя
/// </summary>
/// <param name="registryHolderId">Идентификатор владельца счета</param>
/// <param name="accountTypeId">Идентификатор типа счета</param>
/// <param name="currencyId">Идентификатор валюты</param>
/// <param name="bankId">Идентификатор банка</param>
/// <param name="name">Название счета</param>
/// <param name="isIncludeInBalance">Включать ли счет в общий баланс</param>
/// <param name="isDefault">Является ли счет по умолчанию</param>
/// <param name="isArchived">Архивирован ли счет</param>
/// <param name="creditLimit">Кредитный лимит счета</param>
public class Account(
    Guid registryHolderId,
    Guid accountTypeId,
    Guid currencyId,
    Guid bankId,
    string name,
    bool isIncludeInBalance,
    bool isDefault,
    bool isArchived = false,
    decimal? creditLimit = null) : SoftDeletableEntity
{
    /// <summary>
    /// Идентификатор владельца счета
    /// </summary>
    public Guid RegistryHolderId { get; set; } = registryHolderId;
    
    /// <summary>
    /// Владелец счета
    /// </summary>
    public RegistryHolder RegistryHolder { get; set; } = null!;

    /// <summary>
    /// Идентификатор типа счета
    /// </summary>
    public Guid AccountTypeId { get; set; } = accountTypeId;
    
    /// <summary>
    /// Тип счета
    /// </summary>
    public AccountType AccountType { get; set; } = null!;

    /// <summary>
    /// Идентификатор валюты счета
    /// </summary>
    
    public Guid CurrencyId { get; set; } = currencyId;
    
    /// <summary>
    /// Валюта счета
    /// </summary>
    public Currency Currency { get; set; } = null!;

    /// <summary>
    /// Идентификатор банка
    /// </summary>
    public Guid BankId { get; set; } = bankId;
    
    /// <summary>
    /// Банк, в котором открыт счет
    /// </summary>
    public Bank Bank { get; set; } = null!;

    /// <summary>
    /// Название счета
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Флаг, указывающий, включать ли данный счет в расчет общего баланса
    /// </summary>
    public bool IsIncludeInBalance { get; set; } = isIncludeInBalance;

    /// <summary>
    /// Флаг, указывающий, является ли счет счетом по умолчанию
    /// </summary>
    public bool IsDefault { get; set; } = isDefault;

    /// <summary>
    /// Флаг архивирования счета
    /// </summary>
    public bool IsArchived { get; set; } = isArchived;

    /// <summary>
    /// Кредитный лимит счета (если применимо)
    /// </summary>
    public decimal? CreditLimit { get; set; } = creditLimit;
    
    /// <summary>
    /// Устанавливает счет как счет по умолчанию, устанавливая флаг <see cref="IsDefault"/> в значение <c>true</c>.
    /// </summary>
    public void SetAsDefault() => IsDefault = true;
    
    /// <summary>
    /// Снимает признак счета по умолчанию, устанавливая флаг <see cref="IsDefault"/> в значение <c>false</c>.
    /// </summary>
    public void UnsetAsDefault() => IsDefault = false;
    
    /// <summary>
    /// Архивирует счет, устанавливая флаг <see cref="IsArchived"/> в значение <c>true</c>.
    /// </summary>
    public void Archive() => IsArchived = true;
    
    /// <summary>
    /// Разархивирует счет, устанавливая флаг <see cref="IsArchived"/> в значение <c>false</c>.
    /// </summary>
    public void UnArchive() => IsArchived = false;
}