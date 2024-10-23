using FinanceManager.Core.DataTransferObjects;
using FinanceManager.Core.Services.Abstractions;

namespace FinanceManager.Core.Services
{
    /// <summary>
    /// Service for Expense management (CRUD etc.)
    /// </summary>
    public class ExpenseManager(IExpenseRepository expenseRepository, IUnitOfWork unitOfWork)
    {
        public async Task PutExpense(
            PutExpenseRequestDto command)
        {
            if (command.Id == 0)
            {
                expenseRepository.Add(command.ToModel());
            }
            else
            {
                var expense = await expenseRepository.GetById(command.Id);
                if (expense is null)
                    throw new ApplicationException($"Expense with id {command.Id} was not found");
                expense.Amount = command.Amount;
                expenseRepository.Update(expense);
            }
            await unitOfWork.Commit();
        }

        public async Task<ExpenseDto[]> GetExpenses()
        {
            return await expenseRepository.GetAll(
                selector: x => x.ToDto());
        }

        public async Task DeleteExpense(long id)
        {
            var expense = await expenseRepository.GetById(id);
            if (expense is null)
                throw new ApplicationException($"Expense with id {id} was not found");
            expenseRepository.Delete(expense);
            await unitOfWork.Commit();
        }
    }
}
