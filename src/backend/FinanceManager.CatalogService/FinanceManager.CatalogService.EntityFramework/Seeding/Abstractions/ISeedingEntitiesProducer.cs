using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;

/// <summary>
/// Определяет контракт для источника данных сидинга сущностей типа <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Тип сущности, производной от <see cref="IdentityModel"/>.</typeparam>
public interface ISeedingEntitiesProducer<T>
    where T : IdentityModel
{
    /// <summary>
    /// Загружает коллекцию сущностей из заданного источника.
    /// </summary>
    /// <param name="seedingDataFile">Имя файла с исходными данными для сидинга.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Асинхронная операция, возвращающая массив сущностей <typeparamref name="T"/>.</returns>
    Task<T[]> ProduceEntitiesAsync(string seedingDataFile, CancellationToken cancellationToken = default);
}