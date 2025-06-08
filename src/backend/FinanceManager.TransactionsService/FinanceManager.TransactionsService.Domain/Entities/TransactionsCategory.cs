using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

public class TransactionsCategory(Guid holderId,bool income, bool expense):IdentityModel
{
    //public Guid ParentId { get; set; }
    //public TransactionsCategory Parent { get; set; } = null!;
    public Guid HolderId { get; set; } = holderId;
    public TransactionHolder Holder { get; set; } = null!;
    public bool Income { get; set; } = income;
    public bool Expense { get; set; } = expense;

}