using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.Countries;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;
using Serilog;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Seeders;

public class CountrySeeder(ICountryRepository countryRepository, ILogger logger)
    : DataSeederBase<Country, CreateCountryDto>(logger), IDataSeeder
{
    private const string CountriesSeedingJsonFileName = "countries.json";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedDataAsync(countryRepository, CountriesSeedingJsonFileName, cancellationToken);
    }

    private protected override Func<CreateCountryDto, Country> MapFromDto() =>
        c => c.ToCountry();
}