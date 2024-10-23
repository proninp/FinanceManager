using FinanceManager.Core.Models;
using FinanceManager.Core.Options;
using FinanceManager.Core.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FinanceManager.Infrastructure.Data
{
    public class AppDbContext : DbContext, IUnitOfWork
    {
        private readonly IOptionsSnapshot<DbOptions> options;

        public AppDbContext(IOptionsSnapshot<DbOptions> options)
        {
            this.options = options;
        }

        public DbSet<Expense> Expenses { get; set; }

        public async Task<int> Commit(CancellationToken cancellationToken = default)
        {
            return await SaveChangesAsync(cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(options.Value.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}