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
        [FromServices] ExpenseService expenseService, // expenseService comes from DI container
        [FromBody] PutExpenseRequestDto body)
    {
        await expenseService.PutExpense(body);
    }
}
