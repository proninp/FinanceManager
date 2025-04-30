using FinanceManager.Core.Domain.Abstractions;

namespace FinanceManager.Core.Domain.Entities;

public class Transaction(Guid accountId, Guid categoryId, decimal amount, string? description = null) : IdentityEntity
{
    public Guid AccountId { get; set; } = accountId;
    
    public Account Account { get; set; } = null!;
    
    public Guid CategoryId { get; set; } = categoryId;
    
    public Category Category { get; set; } = null!;
    
    public decimal Amount { get; set; } = amount;

    public string? Description { get; set; } = description;
}