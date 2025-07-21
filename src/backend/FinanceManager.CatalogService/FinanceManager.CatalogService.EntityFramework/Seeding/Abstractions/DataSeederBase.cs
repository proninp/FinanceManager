using System.Text.Json;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Domain.Abstractions;
using Serilog;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;

public abstract class DataSeederBase<T, TCreateDto>(ILogger logger)
    where T : IdentityModel
{
    protected async Task SeedDataAsync(IInitializerRepository<T> repository, string seedingDataFile,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Starting seeding for {EntityType} from {FileName}",
            typeof(T).Name, seedingDataFile);

        if (!await repository.IsEmptyAsync(cancellationToken))
        {
            logger.Information("{EntityType} already has data, skipping seeding", typeof(T).Name);
            return;
        }

        try
        {
            var entities = await LoadEntitiesFromFileAsync(
                MapFromDto(), seedingDataFile, cancellationToken);
            var entitiesCollection = entities as ICollection<T> ?? entities.ToList();
            if (entitiesCollection.Count == 0)
            {
                logger.Warning("No entities with {EntityType} loaded from {FileName}",
                    typeof(T).Name, seedingDataFile);
                return;
            }

            await repository.InitializeAsync(entitiesCollection, cancellationToken);
            logger.Information("Successfully seeded {Count} {EntityType} entities",
                entitiesCollection.Count, typeof(T).Name);
        }
        catch (OperationCanceledException)
        {
            logger.Information("Seeding operation for {EntityType} was cancelled", typeof(T).Name);
            throw;
        }
        catch (FileNotFoundException ex)
        {
            logger.Error("Seeding file not found: {FileName} for {EntityType}", seedingDataFile, typeof(T).Name);
            throw;
        }
        catch (JsonException ex)
        {
            logger.Error(ex, "Invalid JSON format in seeding file {FileName} for {EntityType}. " +
                             "Check file structure and data types", seedingDataFile, typeof(T).Name);
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Unexpected error during seeding {EntityType} from {FileName}",
                typeof(T).Name, seedingDataFile);
            throw;
        }
    }

    private protected abstract Func<TCreateDto, T> MapFromDto();

    private async Task<IEnumerable<T>> LoadEntitiesFromFileAsync(Func<TCreateDto, T> selector, string seedingDataFile,
        CancellationToken cancellationToken = default)
    {
        var jsonPath = Path.Combine("Seeding", "Data", seedingDataFile);
        if (!File.Exists(jsonPath))
        {
            logger.Warning("Seeding file not found: {FilePath}", jsonPath);
            return [];
        }

        var json = await File.ReadAllTextAsync(jsonPath, cancellationToken);
        var models = JsonSerializer.Deserialize<TCreateDto[]>(json);
        logger.Debug("Seeding file deserialized successfully. Deserialized entities count: {EntitiesCount}",
            models?.Length ?? 0);
        return models is not null ? models.Select(selector) : [];
    }
}