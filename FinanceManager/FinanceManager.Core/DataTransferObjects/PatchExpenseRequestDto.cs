using FinanceManager.Core.Models;

namespace FinanceManager.Core.DataTransferObjects
{
    public class PatchExpenseRequestDto
    {
        public long? Id { get; init; }

        public required decimal Amount { get; init; }

        public required DateOnly Date {  get; init; }

        public Expense ToModel()
        {
            return new Expense 
            {
                Id = Id ?? 0,
                Amount = Amount,
                Date = Date
            };
        }
    }
}