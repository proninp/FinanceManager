using FinanceManager.Core.Domain.Abstractions;

namespace FinanceManager.Core.Domain.Entities;

public class Account(
    Guid userId,
    Guid accountTypeId,
    Guid currencyId,
    Guid bankId,
    string name,
    decimal? creditLimit = null,
    bool includeInBalance = false,
    bool isDefault = false,
    bool isArchived = false,
    bool isDeleted = false)  : IdentityEntity
{
    public Guid UserId { get; set; } = userId;

    public User User { get; set; } = null!;

    public Guid AccountTypeId { get; set; } = accountTypeId;

    public AccountType AccountType { get; set; } = null!;

    public Guid CurrencyId { get; set; } = currencyId;

    public Currency Currency { get; set; } = null!;

    public Guid BankId { get; set; } = bankId;

    public Bank Bank { get; set; } = null!;

    public string Name { get; set; } = name;

    public decimal? CreditLimit { get; set; } = creditLimit;
    
    public bool IsIncludeInBalance { get; set; } = includeInBalance;
    
    public bool IsDefault { get; set; } = isDefault;
    
    public bool IsArchived { get; set; } = isArchived;
    
    public bool IsDeleted { get; set; } = isDeleted;
}