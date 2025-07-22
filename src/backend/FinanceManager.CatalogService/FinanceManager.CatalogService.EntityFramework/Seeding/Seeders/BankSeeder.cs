using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;
using Serilog;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Seeders;

/// <summary>
/// Сидер банков. Загружает данные банков из JSON-файла в репозиторий.
/// </summary>
public class BankSeeder(IBankRepository bankRepository, ILogger logger) : 
    DataSeederBase<Bank>(logger), IDataSeeder
{
    private const string BankSeedingJsonFileName = "banks.json";

    /// <summary>
    /// Выполняет загрузку данных банков.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedDataAsync(bankRepository, BankSeedingJsonFileName, cancellationToken);
    }
}