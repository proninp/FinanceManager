using FinanceManager.Core.Models;

namespace FinanceManager.Core.Services.Abstractions;

public interface IExpenseRepository
{
    void Add(Expense expense);

    void Delete(Expense expense);

    void Update(Expense expense);

    Task<TResult[]> GetAll<TResult>(Func<Expense, TResult> selector);

    Task<Expense?> GetById(long id);
}
