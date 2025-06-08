using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

public class Transfer(
    Guid fromAccountId,
    Guid toAccountId,
    decimal fromAmount,
    decimal toAmount,
    string? description = null) : IdentityModel
{
    public Guid FromAccountId { get; set; } = fromAccountId;
    public TransactionsAccount FromAccount { get; set; } = null!;
    public Guid ToAccountId { get; set; } = toAccountId;
    public TransactionsAccount ToAccount { get; set; } = null!;
    public decimal FromAmount { get; set; } = fromAmount;
    public decimal ToAmount { get; set; } = toAmount;
    public string? Description { get; set; } = description;
}