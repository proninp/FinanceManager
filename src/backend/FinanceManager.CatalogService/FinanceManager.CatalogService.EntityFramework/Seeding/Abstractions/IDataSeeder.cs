using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;

public interface IDataSeeder<T>
    where T : IdentityModel
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}