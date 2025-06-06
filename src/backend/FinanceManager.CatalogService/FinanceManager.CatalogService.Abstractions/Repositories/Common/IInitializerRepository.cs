using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Abstractions.Repositories.Common;

/// <summary>
/// Интерфейс репозитория для инициализации и управления статичными справочниками
/// </summary>
public interface IInitializerRepository<in T> where T : IdentityModel
{
    /// <summary>
    /// Проверяет, является ли справочник пустым
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если справочник пустой</returns>
    Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Инициализирует справочник базовым набором
    /// </summary>
    /// <param name="entities">Список валют для инициализации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Количество добавленных валют</returns>
    Task<int> InitializeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
}