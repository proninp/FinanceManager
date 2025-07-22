using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;
using Serilog;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Seeders;

/// <summary>
/// Сидер валют. Загружает данные валют из JSON-файла в репозиторий.
/// </summary>
public class CurrencyFileSeeder(
    ICurrencyRepository currencyRepository,
    ISeedingEntitiesProducer<Currency> seedingProducer,
    ILogger logger) : FileDataSeederBase<Currency>(seedingProducer, logger), IDataSeeder
{
    private const string CurrencySeedingJsonFileName = "currencies.json";

    /// <summary>
    /// Выполняет загрузку данных валют.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedDataAsync(currencyRepository, CurrencySeedingJsonFileName, cancellationToken);
    }
}