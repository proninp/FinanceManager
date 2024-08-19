using FinanceManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Optional: for configuring entity properties (composition keys etc.)
    /// </summary>
    public sealed class ExpenseConfig : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
        }
    }
}
