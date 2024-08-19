namespace FinanceManager.Core.Models
{
    public class Expense
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
    }
}
