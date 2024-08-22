using FinanceManager.Core.DataTransferObjects;
using FinanceManager.Core.Services.Abstractions;

namespace FinanceManager.Core.Services
{
    public class ExpenseService(IExpenseRepository expenseRepository)
    {
        public async Task PutExpense(
            PutExpenseRequestDto command)
        {
            await expenseRepository.Add(command.ToModel());
        }
    }
}
