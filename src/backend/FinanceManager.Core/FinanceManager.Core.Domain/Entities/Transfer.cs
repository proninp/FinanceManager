using FinanceManager.Core.Domain.Abstractions;

namespace FinanceManager.Core.Domain.Entities;

public class Transfer(Guid fromAccountId, Guid toAccountId, decimal fromAmount, decimal toAmount, string? description)
    : IdentityEntity
{
    public Guid FromAccountId { get; set; } = fromAccountId;

    public Account FromAccount { get; set; } = null!;

    public Guid ToAccountId { get; set; } = toAccountId;

    public Account ToAccount { get; set; } = null!;

    public decimal FromAmount { get; set; } = fromAmount;

    public decimal ToAmount { get; set; } = toAmount;

    public string? Description { get; set; } = description;
}