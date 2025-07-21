using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}