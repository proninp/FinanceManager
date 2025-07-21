using System.Text.Json;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;

public abstract class DataSeederBase<T, TCreateDto>
    where T : IdentityModel
{
    protected async Task SeedDataAsync(IInitializerRepository<T> repository, string seedingDataFile,
        CancellationToken cancellationToken = default)
    {
        if (!await repository.IsEmptyAsync(cancellationToken))
            return;

        var entities = await GetDefaultEntities(GetSelector(), seedingDataFile, cancellationToken);
        await repository.InitializeAsync(entities, cancellationToken);
    }

    private protected abstract Func<TCreateDto, T> GetSelector();

    private static async Task<IEnumerable<T>> GetDefaultEntities(Func<TCreateDto, T> selector, string seedingDataFile,
        CancellationToken cancellationToken = default)
    {
        var jsonPath = Path.Combine("Seeding", "Data", seedingDataFile);
        var json = await File.ReadAllTextAsync(jsonPath, cancellationToken);
        try
        {
            var models = JsonSerializer.Deserialize<TCreateDto[]>(json);
            return models is not null ? models.Select(selector) : [];
        }
        catch (JsonException e)
        {
            // TODO Logging
            throw;
        }
        catch (Exception e)
        {
            // TODO Logging
            throw;
        }
    }
}