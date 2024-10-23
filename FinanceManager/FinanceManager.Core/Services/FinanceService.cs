using FinanceManager.Core.DataTransferObjects;

namespace FinanceManager.Core.Services;

public class FinanceService(ExpenseManager expenseService)
{
    public async Task<FinanceViewModel> GetFinanceData()
    {
        var expenses = await expenseService.GetExpenses();
        return new FinanceViewModel
        { 
            Expenses = expenses
            // categories
            // expenses count
            // etc.
        };
    }
}
