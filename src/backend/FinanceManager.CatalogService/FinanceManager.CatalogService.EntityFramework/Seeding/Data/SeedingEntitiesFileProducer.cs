using System.Text.Json;
using FinanceManager.CatalogService.Domain.Abstractions;
using FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;
using Serilog;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Data;

/// <summary>
/// Реализация <see cref="ISeedingEntitiesProducer{T}"/>, которая считывает данные из JSON-файла,
/// расположенного в каталоге "Seeding/Data/Resources" относительно базовой директории приложения.
/// </summary>
/// <typeparam name="T">Тип сущности, производной от <see cref="IdentityModel"/>.</typeparam>
public class SeedingEntitiesFileProducer<T>(ILogger logger) : ISeedingEntitiesProducer<T>
    where T : IdentityModel
{
    /// <summary>
    /// Загружает сущности из JSON-файла.
    /// </summary>
    /// <param name="seedingDataFile">Имя JSON-файла.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция сущностей из файла.</returns>
    public async Task<T[]> ProduceEntitiesAsync(string seedingDataFile, CancellationToken cancellationToken = default)
    {
        var basePath = AppContext.BaseDirectory;
        var jsonPath = Path.Combine(basePath, "Seeding", "Data", "Resources", seedingDataFile);
        if (!File.Exists(jsonPath))
        {
            logger.Warning("Файл сидинга не найден: {FilePath}", jsonPath);
            return [];
        }

        var json = await File.ReadAllTextAsync(jsonPath, cancellationToken);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var models = JsonSerializer.Deserialize<T[]>(json, options);
        logger.Debug("Файл сидинга успешно десериализован. Количество сущностей: {EntitiesCount}",
            models?.Length ?? 0);
        return models ?? [];
    }
}