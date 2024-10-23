using FinanceManager.Core.Models;
using FinanceManager.Core.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Infrastructure.Data.Repositories;

public class ExpenseRepository(AppDbContext dbContext) : IExpenseRepository
{
    public async Task<TResult[]> GetAll<TResult>(Func<Expense, TResult> selector)
    {
        return await dbContext
            .Expenses
            .AsNoTracking()
            .OrderBy(e => e.Date)
            .Select(x => selector(x))
            .ToArrayAsync();
    }

    public async Task<Expense?> GetById(long id)
    {
        return await dbContext
            .Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public void Add(Expense expense)
    {
        dbContext.Add(expense);
    }

    public void Delete(Expense expense)
    {
        dbContext.Expenses.Remove(expense);
    }

    public void Update(Expense expense)
    {
        dbContext.Expenses.Update(expense);
    }
    // TODO сделать операцию вывода ToList с сортировкой, update по id и удаление записей по id
}
