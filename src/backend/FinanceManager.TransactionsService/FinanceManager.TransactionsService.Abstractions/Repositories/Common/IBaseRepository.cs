using System.Linq.Expressions;
using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Abstractions.Repositories.Common;

/// <summary>
/// Базовый интерфейс репозитория для работы с сущностями
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
/// <typeparam name="TFilterDto">Тип DTO для фильтрации</typeparam>
public interface IBaseRepository<T, in TFilterDto> where T:IdentityModel
{
    /// <summary>
    /// Проверяет существование сущности по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если сущность существует</returns>
    Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно получает сущность по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="includeRelated">Включать ли связанные сущности</param>
    /// <param name="disableTracking">Отключить ли отслеживание изменений</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Сущность или null, если не найдена</returns>
    Task<T?> GetByIdAsync(Guid id, bool includeRelated = true, bool disableTracking = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список сущностей с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации и пагинации</param>
    /// <param name="includeRelated">Включать ли связанные сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список сущностей и общее количество</returns>
    Task<ICollection<T>> GetPagedAsync(
        TFilterDto filter,
        bool includeRelated = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новую сущность
    /// </summary>
    /// <param name="entity">Сущность для добавления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Добавленная сущность</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую сущность
    /// </summary>
    /// <param name="entity">Сущность для обновления</param>
    /// <returns>Обновленная сущность</returns>
    T Update(T entity);

    /// <summary>
    /// Частично обновляет сущность (только указанные свойства)
    /// </summary>
    /// <param name="entity">Сущность для обновления</param>
    /// <param name="properties">Свойства для обновления</param>
    T UpdatePartial(T entity, params Expression<Func<T, object>>[] properties);

    /// <summary>
    /// Удаляет сущность
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если сущность была удалена</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}