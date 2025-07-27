using System.Linq.Expressions;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Abstractions;
using FinanceManager.CatalogService.Domain.Abstractions;
using FinanceManager.CatalogService.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.CatalogService.Repositories.Abstractions;

/// <summary>
/// Базовый репозиторий для работы с сущностями, поддерживающий пагинацию и фильтрацию.
/// </summary>
/// <typeparam name="T">Тип сущности.</typeparam>
/// <typeparam name="TFilterDto">Тип DTO фильтра для пагинации.</typeparam>
public abstract class BaseRepository<T, TFilterDto>(DatabaseContext context) : IBaseRepository<T, TFilterDto>
    where T : IdentityModel
    where TFilterDto : BasePaginationDto
{
    private protected readonly DbSet<T> Entities = context.Set<T>();
    
    /// <summary>
    /// Проверяет, является ли сущность пустой
    /// </summary>
    /// <param name="cancellationToken">Токен отмены для прерывания операции</param>
    /// <returns>
    /// Задача, которая завершается с результатом:
    /// <c>true</c> - если сущность пуста;
    /// <c>false</c> - если содержит элементы
    /// </returns>
    public async Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default) =>
        !await Entities.AnyAsync(cancellationToken);

    /// <summary>
    /// Проверяет, существует ли сущность с указанным идентификатором.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если сущность существует, иначе false.</returns>
    public async Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Entities
            .AnyAsync(e => e.Id == id, cancellationToken);
    }

    /// <summary>
    /// Получает сущность по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="includeRelated">Включать связанные сущности.</param>
    /// <param name="disableTracking">Отключить отслеживание изменений.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сущность или null, если не найдена.</returns>
    public async Task<T?> GetByIdAsync(Guid id, bool includeRelated = true, bool disableTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsQueryable();
        if (disableTracking)
            query = query.AsNoTrackingWithIdentityResolution();

        if (includeRelated)
            query = IncludeRelatedEntities(query);

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    /// <summary>
    /// Включает связанные сущности в запрос.
    /// </summary>
    /// <param name="query">Исходный запрос.</param>
    /// <returns>Запрос с включёнными связанными сущностями.</returns>
    private protected virtual IQueryable<T> IncludeRelatedEntities(IQueryable<T> query) =>
        query;

    /// <summary>
    /// Получает страницу сущностей с применением фильтрации и пагинации.
    /// </summary>
    /// <param name="filter">Фильтр и параметры пагинации.</param>
    /// <param name="includeRelated">Включать связанные сущности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция сущностей.</returns>
    public async Task<ICollection<T>> GetPagedAsync(TFilterDto filter, bool includeRelated = true,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsNoTracking();

        query = SetFilters(filter, query);

        query = query.Skip(filter.Skip).Take(filter.Take);

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Применяет фильтры к запросу.
    /// </summary>
    /// <param name="filter">Фильтр.</param>
    /// <param name="query">Исходный запрос.</param>
    /// <returns>Запрос с применёнными фильтрами.</returns>
    private protected abstract IQueryable<T> SetFilters(TFilterDto filter, IQueryable<T> query);

    /// <summary>
    /// Добавляет новую сущность в набор.
    /// </summary>
    /// <param name="entity">Добавляемая сущность.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Добавленная сущность.</returns>
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var addedEntity = await Entities.AddAsync(entity, cancellationToken);
        return addedEntity.Entity;
    }

    /// <summary>
    /// Обновляет сущность.
    /// </summary>
    /// <param name="entity">Обновляемая сущность.</param>
    /// <returns>Обновлённая сущность.</returns>
    public T Update(T entity)
    {
        return Entities.Update(entity).Entity;
    }

    /// <summary>
    /// Удаляет сущность по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если сущность была удалена, иначе false.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await Entities
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        return result > 0;
    }

    /// <summary>
    /// Проверяет, является ли запись уникальной в соответствии с заданным предикатом.
    /// </summary>
    /// <param name="query">Запрос, к которому применяется проверка.</param>
    /// <param name="predicate">Условие, определяющее уникальность записи.</param>
    /// <param name="excludeId">
    /// Идентификатор записи, которую нужно исключить из проверки (например, при обновлении записи).
    /// </param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Возвращает true, если запись уникальна (не существует других записей, удовлетворяющих предикату),
    /// иначе false.
    /// </returns>
    protected async Task<bool> IsUniqueAsync(IQueryable<T> query, Expression<Func<T, bool>> predicate,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        if (excludeId.HasValue)
            query = query.Where(e => e.Id != excludeId.Value);

        return !await query.AnyAsync(predicate, cancellationToken: cancellationToken);
    }
}