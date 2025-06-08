using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

public class Transaction(Guid accountId, Guid categoryId, decimal amount, string? description = null) : IdentityModel
{
    public Guid AccountId { get; set; } = accountId;
    public TransactionsAccount Account { get; set; } = null!;
    public Guid CategoryId { get; set; } = categoryId;
    public TransactionsCategory Category { get; set; } = null!;
    public decimal Amount { get; set; } = amount;
    public string? Description { get; set; } = description;
}