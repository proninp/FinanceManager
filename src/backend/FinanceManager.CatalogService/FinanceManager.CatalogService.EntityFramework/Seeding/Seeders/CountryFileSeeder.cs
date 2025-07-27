using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;
using Serilog;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Seeders;

/// <summary>
/// Сидер стран. Загружает данные стран из JSON-файла в репозиторий.
/// </summary>
public class CountryFileSeeder(
    ICountryRepository countryRepository,
    ISeedingEntitiesProducer<Country> seedingProducer,
    ILogger logger)
    : FileDataSeederBase<Country>(seedingProducer, logger), IDataSeeder
{
    private const string CountriesSeedingJsonFileName = "countries.json";

    /// <summary>
    /// Выполняет загрузку данных стран.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedDataAsync(countryRepository, CountriesSeedingJsonFileName, cancellationToken);
    }
}