using FinanceManager.Core.Models;

namespace FinanceManager.Core.DataTransferObjects
{
    public class PutExpenseRequestDto
    {
        public long? Id { get; init; }
        public required decimal Amount { get; init; }

        public Expense ToModel()
        {
            return new Expense { Id = Id ?? 0, Amount = Amount };
        }
    }
}