using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

public class Account(
    Guid registryHolderId,
    Guid accountTypeId,
    Guid currencyId,
    Guid bankId,
    string name,
    bool isIncludeInBalance,
    bool isDefault,
    bool isArchived = false,
    bool isDeleted = false,
    decimal? creditLimit = null) : IdentityModel
{
    public Guid RegistryHolderId { get; set; } = registryHolderId;
    
    public RegistryHolder RegistryHolder { get; set; } = null!;

    public Guid AccountTypeId { get; set; } = accountTypeId;
    
    public AccountType AccountType { get; set; } = null!;

    public Guid CurrencyId { get; set; } = currencyId;
    
    public Currency Currency { get; set; } = null!;

    public Guid BankId { get; set; } = bankId;
    
    public Bank Bank { get; set; } = null!;

    public string Name { get; set; } = name;

    public bool IsIncludeInBalance { get; set; } = isIncludeInBalance;

    public bool IsDefault { get; set; } = isDefault;

    public bool IsArchived { get; set; } = isArchived;

    public bool IsDeleted { get; set; } = isDeleted;

    public decimal? CreditLimit { get; set; } = creditLimit;
}