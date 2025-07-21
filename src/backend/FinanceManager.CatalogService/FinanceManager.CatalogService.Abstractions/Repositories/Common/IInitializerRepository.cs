using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Abstractions.Repositories.Common;

/// <summary>
/// Интерфейс репозитория для инициализации и управления статичными справочниками
/// </summary>
public interface IInitializerRepository<in T> where T : IdentityModel
{
    /// <summary>
    /// Инициализирует справочник базовым набором
    /// </summary>
    /// <param name="entities">Список валют для инициализации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Количество добавленных валют</returns>
    Task<int> InitializeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default);
}