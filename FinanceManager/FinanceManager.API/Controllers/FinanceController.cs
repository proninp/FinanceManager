using FinanceManager.Core.DataTransferObjects;
using FinanceManager.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.API.Controllers;

[ApiController]
[Route("finance")]
public class FinanceController : ControllerBase
{
    [HttpPut]
    public async Task PutExpense(
        [FromServices] ExpenseManager expenseService, // expenseService comes from DI container
        [FromBody] PutExpenseRequestDto body)
    {
        await expenseService.PutExpense(body);
    }

    [HttpGet] // API -> Aggregate Service -> Domain Service -> Db Repository
    public async Task<FinanceViewModel> GetExpenses(
        [FromServices] FinanceService financeService)
    {
        return await financeService.GetFinanceData();
    }
}
