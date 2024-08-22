using FinanceManager.Core.Models;

namespace FinanceManager.Core.Services.Abstractions;

public interface IExpenseRepository
{
    Task Add(Expense expense);
}
