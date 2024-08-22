using FinanceManager.Core.Models;
using FinanceManager.Core.Services.Abstractions;

namespace FinanceManager.Infrastructure.Data.Repositories;

public class ExpenseRepository(AppDbContext dbContext) : IExpenseRepository
{
    public async Task Add(Expense expense)
    {
        dbContext.Add(expense);
        await dbContext.SaveChangesAsync();
    }
    // TODO сделать операцию вывода ToList с сортировкой, update по id и удаление записей по id
}
