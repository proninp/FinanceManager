using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;

/// <summary>
/// Интерфейс для реализации сидинга (инициализации) данных.
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// Выполняет инициализацию данных.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task SeedAsync(CancellationToken cancellationToken = default);
}