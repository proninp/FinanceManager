using FinanceManager.Core.Domain.Abstractions;

namespace FinanceManager.Core.Domain.Entities;

public class AccountType(string code, string description, bool isDeleted = false) : IdentityEntity
{
    public string Code { get; set; } = code;

    public string Description { get; set; } = description;
    
    public bool IsDeleted { get; set; } = isDeleted;
}