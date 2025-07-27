using System.Text.Json;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Domain.Abstractions;
using Serilog;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;

/// <summary>
/// Базовый класс для сидеров данных.
/// Загружает и сохраняет данные сущностей в соответствующий репозиторий.
/// </summary>
/// <typeparam name="T">Тип сущности, которая будет загружена.</typeparam>
public abstract class FileDataSeederBase<T>(ISeedingEntitiesProducer<T> seedingProducer, ILogger logger)
    where T : IdentityModel
{
    /// <summary>
    /// Загружает данные из JSON-файла и сохраняет их в репозиторий.
    /// </summary>
    /// <param name="repository">Репозиторий для инициализации данных.</param>
    /// <param name="seedingDataFile">Имя JSON-файла с данными.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    protected async Task SeedDataAsync(IInitializerRepository<T> repository, string seedingDataFile,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Запуск сидинга для {EntityType} из файла {FileName}",
            typeof(T).Name, seedingDataFile);

        if (!await repository.IsEmptyAsync(cancellationToken))
        {
            logger.Information("Данные {EntityType} уже существуют. Сидинг пропущен", typeof(T).Name);
            return;
        }

        try
        {
            var entities = await seedingProducer.ProduceEntitiesAsync(seedingDataFile, cancellationToken);
            if (entities.Length == 0)
            {
                logger.Warning("Файл {FileName} не содержит данных для {EntityType}", seedingDataFile, typeof(T).Name);
                return;
            }

            await repository.InitializeAsync(entities, cancellationToken);
            logger.Information("Успешно загружено {Count} сущностей типа {EntityType}",
                entities.Length, typeof(T).Name);
        }
        catch (OperationCanceledException)
        {
            logger.Information("Операция сидинга для {EntityType} была отменена", typeof(T).Name);
            throw;
        }
        catch (FileNotFoundException ex)
        {
            logger.Error("Файл сидинга не найден: {FileName} для {EntityType}", seedingDataFile, typeof(T).Name);
            throw;
        }
        catch (JsonException ex)
        {
            logger.Error(ex, "Неверный формат JSON в файле сидинга {FileName} для {EntityType}. " +
                             "Проверьте структуру и типы данных", seedingDataFile, typeof(T).Name);
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Неожиданная ошибка при сидинге {EntityType} из файла {FileName}",
                typeof(T).Name, seedingDataFile);
            throw;
        }
    }
}