using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.Countries;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Seeders;

public class CountrySeeder(ICountryRepository countryRepository)
    : DataSeederBase<Country, CreateCountryDto>, IDataSeeder<Country>
{
    private const string CountriesSeedingJsonFileName = "countries.json";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedDataAsync(countryRepository, CountriesSeedingJsonFileName, cancellationToken);
    }

    private protected override Func<CreateCountryDto, Country> GetSelector() =>
        c => c.ToCountry();
}