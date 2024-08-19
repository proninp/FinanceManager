using FinanceManager.Core.DataTransferObjects;
using FinanceManager.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Core.Services
{
    public class ExpenseService(AppDbContext dbContext)
    {
        public async Task PutExpense(
            PutExpenseRequestDto command)
        {
            // TODO: добавить сохранение расхода в БД (dbContext)
            throw new NotImplementedException();
        }
    }
}
