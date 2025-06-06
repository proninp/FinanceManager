using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Abstractions.Repositories.Common;

/// <summary>
/// Интерфейс для репозиториев с поддержкой мягкого удаления
/// </summary>
/// <typeparam name="T">Тип сущности с поддержкой мягкого удаления</typeparam>
public interface ISoftDeletableRepository<T> where T : IdentityModel
{
    /// <summary>
    /// Выполняет мягкое удаление сущности
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если сущность была удалена, false если не найдена</returns>
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Восстанавливает мягко удаленную сущность
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если сущность была восстановлена, false если не найдена</returns>
    Task<bool> RestoreAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет существование сущности с учетом мягкого удаления
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="includeDeleted">Включать ли удаленные записи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если сущность существует</returns>
    Task<bool> ExistsAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default);
}