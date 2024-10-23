using FinanceManager.Core.Models;

namespace FinanceManager.Core.DataTransferObjects;

public class ExpenseDto
{
    public long? Id { get; init; }

    public required decimal Amount { get; init; }
    
}
public static class ExpenseMappings
{
    public static ExpenseDto ToDto(this Expense expense)
    {
        return new ExpenseDto { Id = expense.Id, Amount = expense.Amount };
    }
}